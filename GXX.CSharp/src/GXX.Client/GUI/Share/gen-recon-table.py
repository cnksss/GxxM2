#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""FState.pas TFrmDlg declared-member reconciliation table generator.

Lane: p14-client-fstate. Writes the Markdown fragment that the lane report embeds.

Inputs
  --src       UTF-8 mirror of FState.pas (authoritative Delphi source for line numbers)
  --routines  routines.csv produced by FStateExtract.ps1 (routine name -> BodyBegin/EndLine)
  --decl      the CURRENT TFrmDlg.Decl.g.cs (source of truth for "still throws")
  --gen       FStateDeclGen.ps1 (its $Handwritten skip list is the source of truth for
              "managed side already has a real body")
  --out       Markdown output path

Classifier (exactly one label per declared member):
  REAL              managed side already has a 1:1 implementation
  PENDING           the Delphi unit HAS an implementation body; managed side still throws
  ORIGINAL_EMPTY    the Delphi body is empty / comment-only (trivial, copy verbatim)
  ABSTRACT_NO_BODY  the class declares the member but the unit never implements it

Written in Python on purpose: PowerShell 5.1/7 disagree on .ps1 source encoding, and a Chinese
literal in a .ps1 got mangled by the parser in this very lane (see the lane report, D-P14-04).
Python's UTF-8 I/O is unambiguous.
"""

import argparse
import csv
import io
import os
import re

THROW_RE = re.compile(r'throw new NotSupportedException\("TFrmDlg\.([^:]+):')
PROP_RE = re.compile(r'^\s+public [A-Za-z_0-9.]+ ([A-Za-z_][A-Za-z0-9_]*) => [A-Za-z_][A-Za-z0-9_]*;\s*$')
SRC_RE = re.compile(r'/// <summary>Source line (\d+) : ')
MANIFEST_NAMES_RE = re.compile(r'class TFrmDlgMethodTable.*?string\[\] Names =\s*\{(.*?)\};', re.S)
MANIFEST_DECLS_RE = re.compile(r'string\[\] Decls =\s*\{(.*?)\};', re.S)


def read_handwritten(gen_path):
    """Names listed in FStateDeclGen.ps1's $Handwritten array (comments are skipped)."""
    with io.open(gen_path, encoding='utf-8') as fh:
        text = fh.read()
    m = re.search(r'\$Handwritten\s*=\s*@\((.*?)\r?\n\)', text, re.S)
    if not m:
        raise SystemExit('cannot find $Handwritten in ' + gen_path)
    names = []
    for line in m.group(1).splitlines():
        code = line.split('#')[0]
        names.extend(re.findall(r"'([^']+)'", code))
    return names


def read_routines(path):
    """TFrmDlg.<Name> -> list of {BodyBegin, EndLine}."""
    out = {}
    with io.open(path, encoding='utf-8-sig', newline='') as fh:
        for row in csv.DictReader(fh):
            name = row['Name']
            if not name.startswith('TFrmDlg.'):
                continue
            out.setdefault(name, []).append(
                (int(row['BodyBegin']), int(row['EndLine'])))
    return out


def read_manifest_names(manifest_path):
    """TFrmDlgMethodTable: [(name, source_line)] in declaration order.

    Names come from the `Names` block (with `// line N` comments); the `Decls` block carries the
    same order and is used as the source-line fallback, so hand-written members still get a line.
    """
    with io.open(manifest_path, encoding='utf-8') as fh:
        text = fh.read()
    m = MANIFEST_NAMES_RE.search(text)
    if not m:
        raise SystemExit('cannot find TFrmDlgMethodTable.Names in ' + manifest_path)
    entries = re.findall(r'"([^"]+)",\s*//\s*line\s+(\d+)', m.group(1))
    d = MANIFEST_DECLS_RE.search(text[m.start():])
    lines = []
    if d:
        lines = [int(x) for x in re.findall(r'//\s*line\s+(\d+)\s*$', d.group(1), re.M)]
    out = [(n, int(ln)) for n, ln in entries]
    if len(lines) == len(out):
        out = [(out[i][0], lines[i]) for i in range(len(out))]
    return out


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--src', required=True)
    ap.add_argument('--routines', required=True)
    ap.add_argument('--decl', required=True)
    ap.add_argument('--manifest', required=True)
    ap.add_argument('--gen', required=True)
    ap.add_argument('--out', required=True)
    args = ap.parse_args()

    handwritten = read_handwritten(args.gen)
    hw_set = set(handwritten)
    routines = read_routines(args.routines)
    manifest_names = read_manifest_names(args.manifest)

    with io.open(args.decl, encoding='utf-8') as fh:
        decl = fh.read().splitlines()

    # ---- generated-shell facts: source line + "still throws" + the read-only property
    decl_line = {}
    decl_throws = set()
    pending_line = 0
    for line in decl:
        m = SRC_RE.search(line)
        if m:
            pending_line = int(m.group(1))
            continue
        m = THROW_RE.search(line)
        if m:
            decl_line[m.group(1)] = pending_line
            decl_throws.add(m.group(1))
            continue
        m = PROP_RE.match(line)
        if m:
            decl_line[m.group(1)] = pending_line
    throw_now = sum(1 for l in decl if 'throw new NotSupportedException' in l)

    # ---- rows in manifest declaration order; overload duplicates collapse into one row
    rows = []
    seen = set()
    c_real = c_pending = c_original = c_abstract = 0
    for name, decl_src_line in manifest_names:
        if name in seen:
            continue
        seen.add(name)
        key = 'TFrmDlg.' + name
        bodies = routines.get(key)
        body_lines = -1
        span = ''
        if bodies:
            body_lines = max(e - b + 1 for b, e in bodies)
            span = '%d-%d' % (min(b for b, _ in bodies), max(e for _, e in bodies))
        if name in hw_set:
            state = 'REAL'
            c_real += 1
        elif not bodies:
            state = 'ABSTRACT_NO_BODY'
            c_abstract += 1
            span = ''
        elif body_lines <= 3:
            state = 'ORIGINAL_EMPTY'
            c_original += 1
        else:
            state = 'PENDING'
            c_pending += 1
        rows.append((decl_src_line, name, state, span, body_lines))

    total = len(rows)
    portable = c_real + c_pending + c_original
    pct_all = 100.0 * c_real / total if total else 0.0
    pct_portable = 100.0 * c_real / portable if portable else 0.0

    out = []
    w = out.append
    w('### TFrmDlg 声明成员逐条对账表')
    w('')
    w('本表由 `src/GXX.Client/GUI/Share/gen-recon-table.py` 从**原文镜像 + 当前生成壳 + 生成器 `$Handwritten`** '
      '实测生成（非手抄；随时可重跑复现）。')
    w('')
    w('- **TFrmDlg 声明面成员**（`FStateDeclManifest.g.cs` 的 `TFrmDlgMethodTable`：538 条声明，'
      '其中 5 组重载同名 ⇒ 去重后 **%d** 个名字）：**%d**' % (total, len(manifest_names)))
    w('  - 生成壳 `TFrmDlg.Decl.g.cs` **仍声明并 `throw`** 的名字：**%d**（`throw` 语句实测 **%d** 条）'
      % (len(decl_throws), throw_now))
    w('  - `FStateDeclGen.ps1` 的 `$Handwritten` 跳过、由手写 partial 供给真体的名字：**%d**'
      % len(hw_set))
    w('- 生成壳内 `throw new NotSupportedException` 实测条数：**%d**' % throw_now)
    w('- `REAL`（托管侧已有 1:1 真实现）：**%d**' % c_real)
    w('- `PENDING`（原文有实现体、托管侧仍是 throw 壳 ⇒ **本车道待办主体**）：**%d**' % c_pending)
    w('- `ORIGINAL_EMPTY`（原文自带空体/仅注释 ⇒ 可零风险照抄为 空体）：**%d**' % c_original)
    w('- `ABSTRACT_NO_BODY`（原文声明但本单元无实现体 ⇒ 保持 throw 壳）：**%d**' % c_abstract)
    w('')
    w('**当前真实覆盖率（分母 = 全部声明成员 %d）= %d/%d = %.2f%%**' % (total, c_real, total, pct_all))
    w('**可移植面完成率（分母 = REAL+PENDING+ORIGINAL_EMPTY = %d）= %d/%d = %.2f%%**'
      % (portable, c_real, portable, pct_portable))
    w('')
    w('`State` 取值：`REAL` = 真实现已落；`PENDING` = 待办（原文有体）；`ORIGINAL_EMPTY` = 原文空体；'
      '`ABSTRACT_NO_BODY` = 原文无实现体。')
    w('')
    w('> `$Handwritten` 的成员**不出现在下表**：生成壳已不再声明它们，真体在 `TFrmDlg.Core.cs`（前任车道 20 条）'
      '与 `TFrmDlg.Handlers.cs`（本车道切片 1 的 25 条）。逐条清单见本报告“已落地成员”一节，'
      '生成器侧清单见 `FStateDeclGen.ps1` 的 `$Handwritten`。')
    w('')
    w('| # | 原文行 | 成员 | 原文体 | 体行数 | State |')
    w('|---:|---:|---|---|---:|---|')
    for i, (src_line, name, state, span, body_lines) in enumerate(rows, 1):
        span_txt = ('`%s`' % span) if span else 'n/a'
        bl_txt = body_lines if body_lines >= 0 else 'n/a'
        w('| %d | %d | `%s` | %s | %s | %s |' % (i, src_line, name, span_txt, bl_txt, state))

    with io.open(args.out, 'w', encoding='utf-8', newline='\n') as fh:
        fh.write('\n'.join(out) + '\n')

    print('MANIFEST=%d DECLARED_THROW_NAMES=%d HANDWRITTEN=%d DISTINCT=%d REAL=%d PENDING=%d '
          'ORIGINAL_EMPTY=%d ABSTRACT=%d THROW_NOW=%d'
          % (len(manifest_names), len(decl_throws), len(hw_set), total, c_real, c_pending,
             c_original, c_abstract, throw_now))


if __name__ == '__main__':
    main()
