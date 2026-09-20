// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = TFrmCustomMagic 的**全部 published 组件字段**（原文 :17-616，共 600 个），
// 字段名 1:1、声明顺序 1:1；类型由本车道的接缝类型承接（VirtualTrees.pas / SpinEditEx.pas
// 未移植，见 CustomMagicSeams.cs 顶部说明）。每个字段末尾注释即原文行号。
//
// 与 DFM 的关系：原设计期属性（Left/Top/Width/Height/Caption/Items/Enabled…）由
// CustomMagicForm.cs 的 InitializeComponentSeam() 与 FormCreate() 落地；
// **未复刻**的是 DFM 的父子层级（Controls 树）与像素级位置，已在报告登记。
//
// 覆盖行号（Delphi）：uFrmCustomMagic.pas 17-616
// ============================================================================

namespace GXX.M2Server.Forms.CustomMagic;

public partial class TFrmCustomMagic
{
    /// <summary>原文 uFrmCustomMagic.pas:17</summary>
    public TGroupBoxSeam grpMonster = new();
    /// <summary>原文 uFrmCustomMagic.pas:18</summary>
    public CustomMagicTreeHost vstCustomMagic = new();
    /// <summary>原文 uFrmCustomMagic.pas:19</summary>
    public TPageControlSeam pgcMain = new();
    /// <summary>原文 uFrmCustomMagic.pas:20</summary>
    public TTabSheetSeam tsAttack = new();
    /// <summary>原文 uFrmCustomMagic.pas:21</summary>
    public TTabSheetSeam tsServerAttack = new();
    /// <summary>原文 uFrmCustomMagic.pas:22</summary>
    public TPanelSeam pnlBottom = new();
    /// <summary>原文 uFrmCustomMagic.pas:23</summary>
    public TLabelSeam lbl13 = new();
    /// <summary>原文 uFrmCustomMagic.pas:24</summary>
    public TButtonSeam btnSave = new();
    /// <summary>原文 uFrmCustomMagic.pas:25</summary>
    public TCheckBoxSeam chkSendCustomMagicConfig = new();
    /// <summary>原文 uFrmCustomMagic.pas:26</summary>
    public TButtonSeam btnMakeConfigData = new();
    /// <summary>原文 uFrmCustomMagic.pas:27</summary>
    public TSaveDialogSeam dlgSaveMagics = new();
    /// <summary>原文 uFrmCustomMagic.pas:28</summary>
    public TPageControlSeam pgcClient = new();
    /// <summary>原文 uFrmCustomMagic.pas:29</summary>
    public TTabSheetSeam tsBase = new();
    /// <summary>原文 uFrmCustomMagic.pas:30</summary>
    public TGroupBoxSeam grpClientBaseSetting = new();
    /// <summary>原文 uFrmCustomMagic.pas:31</summary>
    public TLabelSeam Label50 = new();
    /// <summary>原文 uFrmCustomMagic.pas:32</summary>
    public TLabelSeam Label51 = new();
    /// <summary>原文 uFrmCustomMagic.pas:33</summary>
    public TComboBoxSeam cbbClientIconFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:34</summary>
    public TSpinEditExSeam seClientIconIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:35</summary>
    public TGroupBoxSeam GroupBox2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:36</summary>
    public TLabelSeam Label225 = new();
    /// <summary>原文 uFrmCustomMagic.pas:37</summary>
    public TLabelSeam Label226 = new();
    /// <summary>原文 uFrmCustomMagic.pas:38</summary>
    public TLabelSeam lbl3 = new();
    /// <summary>原文 uFrmCustomMagic.pas:39</summary>
    public TLabelSeam Label227 = new();
    /// <summary>原文 uFrmCustomMagic.pas:40</summary>
    public TLabelSeam Label228 = new();
    /// <summary>原文 uFrmCustomMagic.pas:41</summary>
    public TEditSeam edtSound1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:42</summary>
    public TEditSeam edtSound2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:43</summary>
    public TEditSeam edtSound3 = new();
    /// <summary>原文 uFrmCustomMagic.pas:44</summary>
    public TEditSeam edtSound4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:45</summary>
    public TEditSeam edtSound5 = new();
    /// <summary>原文 uFrmCustomMagic.pas:46</summary>
    public TTabSheetSeam tsEffect = new();
    /// <summary>原文 uFrmCustomMagic.pas:47</summary>
    public TGroupBoxSeam grpFly = new();
    /// <summary>原文 uFrmCustomMagic.pas:48</summary>
    public TLabelSeam lbl5 = new();
    /// <summary>原文 uFrmCustomMagic.pas:49</summary>
    public TLabelSeam Label1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:50</summary>
    public TLabelSeam Label2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:51</summary>
    public TLabelSeam Label3 = new();
    /// <summary>原文 uFrmCustomMagic.pas:52</summary>
    public TLabelSeam Label4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:53</summary>
    public TLabelSeam Label5 = new();
    /// <summary>原文 uFrmCustomMagic.pas:54</summary>
    public TLabelSeam Label15 = new();
    /// <summary>原文 uFrmCustomMagic.pas:55</summary>
    public TLabelSeam Label16 = new();
    /// <summary>原文 uFrmCustomMagic.pas:56</summary>
    public TComboBoxSeam cbbClientFlyFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:57</summary>
    public TComboBoxSeam cbbClientFlyDrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:58</summary>
    public TComboBoxSeam cbbClientFlyDirCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:59</summary>
    public TCheckBoxSeam chkClientFlyCalcDir = new();
    /// <summary>原文 uFrmCustomMagic.pas:60</summary>
    public TSpinEditExSeam seClientFlyPlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:61</summary>
    public TSpinEditExSeam seClientFlyEmptyCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:62</summary>
    public TSpinEditExSeam seClientFlyPlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:63</summary>
    public TSpinEditExSeam seClientFlyStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:64</summary>
    public TSpinEditExSeam seClientFlyLightRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:65</summary>
    public TGroupBoxSeam grpSelf = new();
    /// <summary>原文 uFrmCustomMagic.pas:66</summary>
    public TLabelSeam Label17 = new();
    /// <summary>原文 uFrmCustomMagic.pas:67</summary>
    public TLabelSeam Label18 = new();
    /// <summary>原文 uFrmCustomMagic.pas:68</summary>
    public TLabelSeam Label19 = new();
    /// <summary>原文 uFrmCustomMagic.pas:69</summary>
    public TLabelSeam Label20 = new();
    /// <summary>原文 uFrmCustomMagic.pas:70</summary>
    public TLabelSeam Label21 = new();
    /// <summary>原文 uFrmCustomMagic.pas:71</summary>
    public TLabelSeam Label22 = new();
    /// <summary>原文 uFrmCustomMagic.pas:72</summary>
    public TLabelSeam Label24 = new();
    /// <summary>原文 uFrmCustomMagic.pas:73</summary>
    public TLabelSeam Label25 = new();
    /// <summary>原文 uFrmCustomMagic.pas:74</summary>
    public TLabelSeam Label96 = new();
    /// <summary>原文 uFrmCustomMagic.pas:75</summary>
    public TLabelSeam Label99 = new();
    /// <summary>原文 uFrmCustomMagic.pas:76</summary>
    public TComboBoxSeam cbbClientSelfFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:77</summary>
    public TComboBoxSeam cbbClientSelfDrawOrder = new();
    /// <summary>原文 uFrmCustomMagic.pas:78</summary>
    public TSpinEditExSeam seClientSelfPlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:79</summary>
    public TSpinEditExSeam seClientSelfPlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:80</summary>
    public TSpinEditExSeam seClientSelfStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:81</summary>
    public TComboBoxSeam cbbClientSelfDrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:82</summary>
    public TSpinEditExSeam seClientSelfEmptyCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:83</summary>
    public TComboBoxSeam cbbClientSelfDirCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:84</summary>
    public TCheckBoxSeam chkClientSelfPlayDelayAction = new();
    /// <summary>原文 uFrmCustomMagic.pas:85</summary>
    public TSpinEditExSeam seClientSelfLightRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:86</summary>
    public TComboBoxSeam cbbClientSelfDirCalcType = new();
    /// <summary>原文 uFrmCustomMagic.pas:87</summary>
    public TGroupBoxSeam grpTarget = new();
    /// <summary>原文 uFrmCustomMagic.pas:88</summary>
    public TLabelSeam Label37 = new();
    /// <summary>原文 uFrmCustomMagic.pas:89</summary>
    public TLabelSeam Label38 = new();
    /// <summary>原文 uFrmCustomMagic.pas:90</summary>
    public TLabelSeam Label39 = new();
    /// <summary>原文 uFrmCustomMagic.pas:91</summary>
    public TLabelSeam Label40 = new();
    /// <summary>原文 uFrmCustomMagic.pas:92</summary>
    public TLabelSeam Label41 = new();
    /// <summary>原文 uFrmCustomMagic.pas:93</summary>
    public TLabelSeam Label42 = new();
    /// <summary>原文 uFrmCustomMagic.pas:94</summary>
    public TLabelSeam Label43 = new();
    /// <summary>原文 uFrmCustomMagic.pas:95</summary>
    public TLabelSeam Label44 = new();
    /// <summary>原文 uFrmCustomMagic.pas:96</summary>
    public TLabelSeam Label45 = new();
    /// <summary>原文 uFrmCustomMagic.pas:97</summary>
    public TLabelSeam Label47 = new();
    /// <summary>原文 uFrmCustomMagic.pas:98</summary>
    public TBevelSeam bvl1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:99</summary>
    public TComboBoxSeam cbbClientTargetFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:100</summary>
    public TSpinEditExSeam seClientTargetPlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:101</summary>
    public TSpinEditExSeam seClientTargetPlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:102</summary>
    public TSpinEditExSeam seClientTargetStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:103</summary>
    public TComboBoxSeam cbbClientTargetDrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:104</summary>
    public TCheckBoxSeam chkClientTargetMultiPlay = new();
    /// <summary>原文 uFrmCustomMagic.pas:105</summary>
    public TCheckBoxSeam chkClientTargetLockDraw = new();
    /// <summary>原文 uFrmCustomMagic.pas:106</summary>
    public TSpinEditExSeam seClientTargetLightRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:107</summary>
    public TCheckBoxSeam chkClientTargetKeepPlay = new();
    /// <summary>原文 uFrmCustomMagic.pas:108</summary>
    public TSpinEditExSeam seClientTargetKeepTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:109</summary>
    public TSpinEditExSeam seClientTargetKeepAttackRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:110</summary>
    public TSpinEditExSeam seClientTargetKeepAttackInterval = new();
    /// <summary>原文 uFrmCustomMagic.pas:111</summary>
    public TCheckBoxSeam chkClientTargetKeepMultiPlay = new();
    /// <summary>原文 uFrmCustomMagic.pas:112</summary>
    public TGroupBoxSeam grpFlyEff = new();
    /// <summary>原文 uFrmCustomMagic.pas:113</summary>
    public TLabelSeam Label14 = new();
    /// <summary>原文 uFrmCustomMagic.pas:114</summary>
    public TLabelSeam Label48 = new();
    /// <summary>原文 uFrmCustomMagic.pas:115</summary>
    public TLabelSeam Label49 = new();
    /// <summary>原文 uFrmCustomMagic.pas:116</summary>
    public TComboBoxSeam cbbClientFlyEffFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:117</summary>
    public TSpinEditExSeam seClientFlyEffStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:118</summary>
    public TComboBoxSeam cbbClientFlyEffDrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:119</summary>
    public TGroupBoxSeam GroupBox1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:120</summary>
    public TLabelSeam Label26 = new();
    /// <summary>原文 uFrmCustomMagic.pas:121</summary>
    public TLabelSeam Label27 = new();
    /// <summary>原文 uFrmCustomMagic.pas:122</summary>
    public TLabelSeam Label28 = new();
    /// <summary>原文 uFrmCustomMagic.pas:123</summary>
    public TLabelSeam Label29 = new();
    /// <summary>原文 uFrmCustomMagic.pas:124</summary>
    public TLabelSeam Label30 = new();
    /// <summary>原文 uFrmCustomMagic.pas:125</summary>
    public TLabelSeam Label36 = new();
    /// <summary>原文 uFrmCustomMagic.pas:126</summary>
    public TComboBoxSeam cbbClientPreTargetFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:127</summary>
    public TSpinEditExSeam seClientPreTargetPlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:128</summary>
    public TSpinEditExSeam seClientPreTargetPlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:129</summary>
    public TSpinEditExSeam seClientPreTargetStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:130</summary>
    public TComboBoxSeam cbbClientPreTargetDrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:131</summary>
    public TCheckBoxSeam chkClientPreTargetLockDraw = new();
    /// <summary>原文 uFrmCustomMagic.pas:132</summary>
    public TSpinEditExSeam seClientPreTargetLightRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:133</summary>
    public TGroupBoxSeam grpFastMove = new();
    /// <summary>原文 uFrmCustomMagic.pas:134</summary>
    public TLabelSeam Label218 = new();
    /// <summary>原文 uFrmCustomMagic.pas:135</summary>
    public TLabelSeam Label219 = new();
    /// <summary>原文 uFrmCustomMagic.pas:136</summary>
    public TLabelSeam Label220 = new();
    /// <summary>原文 uFrmCustomMagic.pas:137</summary>
    public TLabelSeam Label222 = new();
    /// <summary>原文 uFrmCustomMagic.pas:138</summary>
    public TLabelSeam Label223 = new();
    /// <summary>原文 uFrmCustomMagic.pas:139</summary>
    public TLabelSeam Label224 = new();
    /// <summary>原文 uFrmCustomMagic.pas:140</summary>
    public TLabelSeam Label221 = new();
    /// <summary>原文 uFrmCustomMagic.pas:141</summary>
    public TComboBoxSeam cbbClientFastMoveFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:142</summary>
    public TSpinEditExSeam seClientFastMovePlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:143</summary>
    public TSpinEditExSeam seClientFastMovePlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:144</summary>
    public TSpinEditExSeam seClientFastMoveStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:145</summary>
    public TComboBoxSeam cbbClientFastMoveDrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:146</summary>
    public TSpinEditExSeam seClientFastMoveEmptyCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:147</summary>
    public TCheckBoxSeam chkClientFastMoveCalcDir = new();
    /// <summary>原文 uFrmCustomMagic.pas:148</summary>
    public TSpinEditExSeam seFastMoveLightRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:149</summary>
    public TGroupBoxSeam GroupBox4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:150</summary>
    public TLabelSeam Label229 = new();
    /// <summary>原文 uFrmCustomMagic.pas:151</summary>
    public TLabelSeam Label230 = new();
    /// <summary>原文 uFrmCustomMagic.pas:152</summary>
    public TLabelSeam Label231 = new();
    /// <summary>原文 uFrmCustomMagic.pas:153</summary>
    public TLabelSeam Label233 = new();
    /// <summary>原文 uFrmCustomMagic.pas:154</summary>
    public TLabelSeam Label234 = new();
    /// <summary>原文 uFrmCustomMagic.pas:155</summary>
    public TComboBoxSeam cbbClientSelfKeepFile = new();
    /// <summary>原文 uFrmCustomMagic.pas:156</summary>
    public TSpinEditExSeam seClientSelfKeepPlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:157</summary>
    public TSpinEditExSeam seClientSelfKeepPlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:158</summary>
    public TSpinEditExSeam seClientSelfKeepStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:159</summary>
    public TComboBoxSeam cbbClientSelfKeepDrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:160</summary>
    public TLabelSeam Label232 = new();
    /// <summary>原文 uFrmCustomMagic.pas:161</summary>
    public TSpinEditExSeam seClientSelfKeepTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:162</summary>
    public TLabelSeam Label235 = new();
    /// <summary>原文 uFrmCustomMagic.pas:163</summary>
    public TLabelSeam lbl6 = new();
    /// <summary>原文 uFrmCustomMagic.pas:164</summary>
    public TLabelSeam Label236 = new();
    /// <summary>原文 uFrmCustomMagic.pas:165</summary>
    public TSpinEditExSeam seTargetKeepLightRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:166</summary>
    public TLabelSeam Label334 = new();
    /// <summary>原文 uFrmCustomMagic.pas:167</summary>
    public TLabelSeam Label335 = new();
    /// <summary>原文 uFrmCustomMagic.pas:168</summary>
    public TSpinEditExSeam seClientTargetKeepTime2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:169</summary>
    public TCheckBoxSeam chkClientFastMoveNoHitAction = new();
    /// <summary>原文 uFrmCustomMagic.pas:170</summary>
    public TLabelSeam Label336 = new();
    /// <summary>原文 uFrmCustomMagic.pas:171</summary>
    public TLabelSeam Label337 = new();
    /// <summary>原文 uFrmCustomMagic.pas:172</summary>
    public TSpinEditExSeam seClientSelfKeepTime2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:173</summary>
    public TCheckBoxSeam chkClientSelfPlayFailNoDraw = new();
    /// <summary>原文 uFrmCustomMagic.pas:174</summary>
    public TLabelSeam Label205 = new();
    /// <summary>原文 uFrmCustomMagic.pas:175</summary>
    public TEditSeam edtSound6 = new();
    /// <summary>原文 uFrmCustomMagic.pas:176</summary>
    public TCheckBoxSeam chkClientFlyFireGunMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:177</summary>
    public TLabelSeam lbl1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:178</summary>
    public TLabelSeam Label31 = new();
    /// <summary>原文 uFrmCustomMagic.pas:179</summary>
    public TLabelSeam Label209 = new();
    /// <summary>原文 uFrmCustomMagic.pas:180</summary>
    public TSpinEditExSeam seClientPreTargetStartIndex2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:181</summary>
    public TLabelSeam Label212 = new();
    /// <summary>原文 uFrmCustomMagic.pas:182</summary>
    public TSpinEditExSeam seClientTargetStartIndex2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:183</summary>
    public TLabelSeam Label213 = new();
    /// <summary>原文 uFrmCustomMagic.pas:184</summary>
    public TComboBoxSeam cbbClientPreTargetDrawMode2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:185</summary>
    public TLabelSeam Label237 = new();
    /// <summary>原文 uFrmCustomMagic.pas:186</summary>
    public TComboBoxSeam cbbClientTargetDrawMode2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:187</summary>
    public TLabelSeam Label200 = new();
    /// <summary>原文 uFrmCustomMagic.pas:188</summary>
    public TLabelSeam Label46 = new();
    /// <summary>原文 uFrmCustomMagic.pas:189</summary>
    public TCheckBoxSeam chkSelf_SyncHumAction = new();
    /// <summary>原文 uFrmCustomMagic.pas:190</summary>
    public TPageControlSeam pgcMagicType = new();
    /// <summary>原文 uFrmCustomMagic.pas:191</summary>
    public TTabSheetSeam tsMagicAttack = new();
    /// <summary>原文 uFrmCustomMagic.pas:192</summary>
    public TTabSheetSeam tsMagicProtected = new();
    /// <summary>原文 uFrmCustomMagic.pas:193</summary>
    public TPageControlSeam pgcAttack = new();
    /// <summary>原文 uFrmCustomMagic.pas:194</summary>
    public TTabSheetSeam tsAdditionals = new();
    /// <summary>原文 uFrmCustomMagic.pas:195</summary>
    public TLabelSeam Label60 = new();
    /// <summary>原文 uFrmCustomMagic.pas:196</summary>
    public TLabelSeam Label62 = new();
    /// <summary>原文 uFrmCustomMagic.pas:197</summary>
    public TLabelSeam Label63 = new();
    /// <summary>原文 uFrmCustomMagic.pas:198</summary>
    public TLabelSeam Label64 = new();
    /// <summary>原文 uFrmCustomMagic.pas:199</summary>
    public TLabelSeam Label65 = new();
    /// <summary>原文 uFrmCustomMagic.pas:200</summary>
    public TLabelSeam Label67 = new();
    /// <summary>原文 uFrmCustomMagic.pas:201</summary>
    public TLabelSeam Label68 = new();
    /// <summary>原文 uFrmCustomMagic.pas:202</summary>
    public TLabelSeam Label69 = new();
    /// <summary>原文 uFrmCustomMagic.pas:203</summary>
    public TLabelSeam Label70 = new();
    /// <summary>原文 uFrmCustomMagic.pas:204</summary>
    public TLabelSeam Label71 = new();
    /// <summary>原文 uFrmCustomMagic.pas:205</summary>
    public TLabelSeam Label72 = new();
    /// <summary>原文 uFrmCustomMagic.pas:206</summary>
    public TLabelSeam Label73 = new();
    /// <summary>原文 uFrmCustomMagic.pas:207</summary>
    public TLabelSeam Label74 = new();
    /// <summary>原文 uFrmCustomMagic.pas:208</summary>
    public TLabelSeam Label75 = new();
    /// <summary>原文 uFrmCustomMagic.pas:209</summary>
    public TLabelSeam Label76 = new();
    /// <summary>原文 uFrmCustomMagic.pas:210</summary>
    public TLabelSeam Label77 = new();
    /// <summary>原文 uFrmCustomMagic.pas:211</summary>
    public TLabelSeam Label78 = new();
    /// <summary>原文 uFrmCustomMagic.pas:212</summary>
    public TLabelSeam Label79 = new();
    /// <summary>原文 uFrmCustomMagic.pas:213</summary>
    public TLabelSeam Label80 = new();
    /// <summary>原文 uFrmCustomMagic.pas:214</summary>
    public TLabelSeam Label81 = new();
    /// <summary>原文 uFrmCustomMagic.pas:215</summary>
    public TLabelSeam Label82 = new();
    /// <summary>原文 uFrmCustomMagic.pas:216</summary>
    public TLabelSeam Label83 = new();
    /// <summary>原文 uFrmCustomMagic.pas:217</summary>
    public TLabelSeam Label84 = new();
    /// <summary>原文 uFrmCustomMagic.pas:218</summary>
    public TLabelSeam Label54 = new();
    /// <summary>原文 uFrmCustomMagic.pas:219</summary>
    public TLabelSeam Label55 = new();
    /// <summary>原文 uFrmCustomMagic.pas:220</summary>
    public TLabelSeam Label56 = new();
    /// <summary>原文 uFrmCustomMagic.pas:221</summary>
    public TLabelSeam Label57 = new();
    /// <summary>原文 uFrmCustomMagic.pas:222</summary>
    public TLabelSeam Label59 = new();
    /// <summary>原文 uFrmCustomMagic.pas:223</summary>
    public TLabelSeam Label61 = new();
    /// <summary>原文 uFrmCustomMagic.pas:224</summary>
    public TLabelSeam Label101 = new();
    /// <summary>原文 uFrmCustomMagic.pas:225</summary>
    public TLabelSeam Label102 = new();
    /// <summary>原文 uFrmCustomMagic.pas:226</summary>
    public TLabelSeam Label103 = new();
    /// <summary>原文 uFrmCustomMagic.pas:227</summary>
    public TLabelSeam Label104 = new();
    /// <summary>原文 uFrmCustomMagic.pas:228</summary>
    public TLabelSeam Label105 = new();
    /// <summary>原文 uFrmCustomMagic.pas:229</summary>
    public TLabelSeam Label106 = new();
    /// <summary>原文 uFrmCustomMagic.pas:230</summary>
    public TLabelSeam Label107 = new();
    /// <summary>原文 uFrmCustomMagic.pas:231</summary>
    public TLabelSeam Label108 = new();
    /// <summary>原文 uFrmCustomMagic.pas:232</summary>
    public TLabelSeam Label109 = new();
    /// <summary>原文 uFrmCustomMagic.pas:233</summary>
    public TLabelSeam Label110 = new();
    /// <summary>原文 uFrmCustomMagic.pas:234</summary>
    public TLabelSeam Label111 = new();
    /// <summary>原文 uFrmCustomMagic.pas:235</summary>
    public TLabelSeam Label112 = new();
    /// <summary>原文 uFrmCustomMagic.pas:236</summary>
    public TLabelSeam Label113 = new();
    /// <summary>原文 uFrmCustomMagic.pas:237</summary>
    public TLabelSeam Label114 = new();
    /// <summary>原文 uFrmCustomMagic.pas:238</summary>
    public TLabelSeam Label115 = new();
    /// <summary>原文 uFrmCustomMagic.pas:239</summary>
    public TLabelSeam Label116 = new();
    /// <summary>原文 uFrmCustomMagic.pas:240</summary>
    public TLabelSeam Label117 = new();
    /// <summary>原文 uFrmCustomMagic.pas:241</summary>
    public TLabelSeam Label118 = new();
    /// <summary>原文 uFrmCustomMagic.pas:242</summary>
    public TLabelSeam Label119 = new();
    /// <summary>原文 uFrmCustomMagic.pas:243</summary>
    public TLabelSeam Label120 = new();
    /// <summary>原文 uFrmCustomMagic.pas:244</summary>
    public TLabelSeam Label121 = new();
    /// <summary>原文 uFrmCustomMagic.pas:245</summary>
    public TLabelSeam Label122 = new();
    /// <summary>原文 uFrmCustomMagic.pas:246</summary>
    public TLabelSeam Label123 = new();
    /// <summary>原文 uFrmCustomMagic.pas:247</summary>
    public TLabelSeam Label124 = new();
    /// <summary>原文 uFrmCustomMagic.pas:248</summary>
    public TLabelSeam Label125 = new();
    /// <summary>原文 uFrmCustomMagic.pas:249</summary>
    public TLabelSeam Label126 = new();
    /// <summary>原文 uFrmCustomMagic.pas:250</summary>
    public TLabelSeam Label127 = new();
    /// <summary>原文 uFrmCustomMagic.pas:251</summary>
    public TLabelSeam Label128 = new();
    /// <summary>原文 uFrmCustomMagic.pas:252</summary>
    public TLabelSeam Label129 = new();
    /// <summary>原文 uFrmCustomMagic.pas:253</summary>
    public TLabelSeam Label130 = new();
    /// <summary>原文 uFrmCustomMagic.pas:254</summary>
    public TLabelSeam Label131 = new();
    /// <summary>原文 uFrmCustomMagic.pas:255</summary>
    public TLabelSeam Label132 = new();
    /// <summary>原文 uFrmCustomMagic.pas:256</summary>
    public TLabelSeam Label133 = new();
    /// <summary>原文 uFrmCustomMagic.pas:257</summary>
    public TLabelSeam Label134 = new();
    /// <summary>原文 uFrmCustomMagic.pas:258</summary>
    public TLabelSeam Label135 = new();
    /// <summary>原文 uFrmCustomMagic.pas:259</summary>
    public TLabelSeam Label136 = new();
    /// <summary>原文 uFrmCustomMagic.pas:260</summary>
    public TLabelSeam Label138 = new();
    /// <summary>原文 uFrmCustomMagic.pas:261</summary>
    public TLabelSeam Label139 = new();
    /// <summary>原文 uFrmCustomMagic.pas:262</summary>
    public TLabelSeam Label141 = new();
    /// <summary>原文 uFrmCustomMagic.pas:263</summary>
    public TLabelSeam Label142 = new();
    /// <summary>原文 uFrmCustomMagic.pas:264</summary>
    public TLabelSeam Label143 = new();
    /// <summary>原文 uFrmCustomMagic.pas:265</summary>
    public TLabelSeam Label144 = new();
    /// <summary>原文 uFrmCustomMagic.pas:266</summary>
    public TLabelSeam Label152 = new();
    /// <summary>原文 uFrmCustomMagic.pas:267</summary>
    public TLabelSeam Label154 = new();
    /// <summary>原文 uFrmCustomMagic.pas:268</summary>
    public TLabelSeam Label156 = new();
    /// <summary>原文 uFrmCustomMagic.pas:269</summary>
    public TLabelSeam Label158 = new();
    /// <summary>原文 uFrmCustomMagic.pas:270</summary>
    public TLabelSeam Label162 = new();
    /// <summary>原文 uFrmCustomMagic.pas:271</summary>
    public TLabelSeam Label163 = new();
    /// <summary>原文 uFrmCustomMagic.pas:272</summary>
    public TLabelSeam Label165 = new();
    /// <summary>原文 uFrmCustomMagic.pas:273</summary>
    public TLabelSeam Label168 = new();
    /// <summary>原文 uFrmCustomMagic.pas:274</summary>
    public TLabelSeam Label172 = new();
    /// <summary>原文 uFrmCustomMagic.pas:275</summary>
    public TLabelSeam Label173 = new();
    /// <summary>原文 uFrmCustomMagic.pas:276</summary>
    public TCheckBoxSeam chkAdditional0 = new();
    /// <summary>原文 uFrmCustomMagic.pas:277</summary>
    public TSpinEditExSeam seAdditionalRate0 = new();
    /// <summary>原文 uFrmCustomMagic.pas:278</summary>
    public TSpinEditExSeam seAdditionalTime0 = new();
    /// <summary>原文 uFrmCustomMagic.pas:279</summary>
    public TCheckBoxSeam chkAdditional1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:280</summary>
    public TSpinEditExSeam seAdditionalRate1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:281</summary>
    public TSpinEditExSeam seAdditionalTime1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:282</summary>
    public TCheckBoxSeam chkAdditional2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:283</summary>
    public TSpinEditExSeam seAdditionalRate2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:284</summary>
    public TSpinEditExSeam seAdditionalTime2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:285</summary>
    public TCheckBoxSeam chkAdditional3 = new();
    /// <summary>原文 uFrmCustomMagic.pas:286</summary>
    public TSpinEditExSeam seAdditionalRate3 = new();
    /// <summary>原文 uFrmCustomMagic.pas:287</summary>
    public TSpinEditExSeam seAdditionalTime3 = new();
    /// <summary>原文 uFrmCustomMagic.pas:288</summary>
    public TCheckBoxSeam chkAdditional4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:289</summary>
    public TSpinEditExSeam seAdditionalRate4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:290</summary>
    public TSpinEditExSeam seAdditionalTime4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:291</summary>
    public TCheckBoxSeam chkAdditional5 = new();
    /// <summary>原文 uFrmCustomMagic.pas:292</summary>
    public TSpinEditExSeam seAdditionalRate5 = new();
    /// <summary>原文 uFrmCustomMagic.pas:293</summary>
    public TSpinEditExSeam seAdditionalTime5 = new();
    /// <summary>原文 uFrmCustomMagic.pas:294</summary>
    public TCheckBoxSeam chkAdditional6 = new();
    /// <summary>原文 uFrmCustomMagic.pas:295</summary>
    public TSpinEditExSeam seAdditionalRate6 = new();
    /// <summary>原文 uFrmCustomMagic.pas:296</summary>
    public TSpinEditExSeam seAdditionalTime6 = new();
    /// <summary>原文 uFrmCustomMagic.pas:297</summary>
    public TCheckBoxSeam chkAdditional7 = new();
    /// <summary>原文 uFrmCustomMagic.pas:298</summary>
    public TSpinEditExSeam seAdditionalRate7 = new();
    /// <summary>原文 uFrmCustomMagic.pas:299</summary>
    public TSpinEditExSeam seAdditionalTime7 = new();
    /// <summary>原文 uFrmCustomMagic.pas:300</summary>
    public TCheckBoxSeam chkAdditional8 = new();
    /// <summary>原文 uFrmCustomMagic.pas:301</summary>
    public TSpinEditExSeam seAdditionalRate8 = new();
    /// <summary>原文 uFrmCustomMagic.pas:302</summary>
    public TSpinEditExSeam seAdditionalTime8 = new();
    /// <summary>原文 uFrmCustomMagic.pas:303</summary>
    public TCheckBoxSeam chkAdditional9 = new();
    /// <summary>原文 uFrmCustomMagic.pas:304</summary>
    public TSpinEditExSeam seAdditionalRate9 = new();
    /// <summary>原文 uFrmCustomMagic.pas:305</summary>
    public TSpinEditExSeam seAdditionalTime9 = new();
    /// <summary>原文 uFrmCustomMagic.pas:306</summary>
    public TCheckBoxSeam chkseAdditionaHighLevel4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:307</summary>
    public TSpinEditExSeam seAdditionaHP0 = new();
    /// <summary>原文 uFrmCustomMagic.pas:308</summary>
    public TCheckBoxSeam chkAdditional10 = new();
    /// <summary>原文 uFrmCustomMagic.pas:309</summary>
    public TSpinEditExSeam seAdditionalRate10 = new();
    /// <summary>原文 uFrmCustomMagic.pas:310</summary>
    public TSpinEditExSeam seAdditionalTime10 = new();
    /// <summary>原文 uFrmCustomMagic.pas:311</summary>
    public TSpinEditExSeam seAdditionalRate0_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:312</summary>
    public TSpinEditExSeam seAdditionalRate1_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:313</summary>
    public TSpinEditExSeam seAdditionalRate2_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:314</summary>
    public TSpinEditExSeam seAdditionalRate3_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:315</summary>
    public TSpinEditExSeam seAdditionalRate4_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:316</summary>
    public TSpinEditExSeam seAdditionalRate5_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:317</summary>
    public TSpinEditExSeam seAdditionalRate6_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:318</summary>
    public TSpinEditExSeam seAdditionalRate7_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:319</summary>
    public TSpinEditExSeam seAdditionalRate8_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:320</summary>
    public TSpinEditExSeam seAdditionalRate9_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:321</summary>
    public TSpinEditExSeam seAdditionalRate10_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:322</summary>
    public TSpinEditExSeam seAdditionalTime0_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:323</summary>
    public TSpinEditExSeam seAdditionalTime1_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:324</summary>
    public TSpinEditExSeam seAdditionalTime2_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:325</summary>
    public TSpinEditExSeam seAdditionalTime3_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:326</summary>
    public TSpinEditExSeam seAdditionalTime4_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:327</summary>
    public TSpinEditExSeam seAdditionalTime5_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:328</summary>
    public TSpinEditExSeam seAdditionalTime6_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:329</summary>
    public TSpinEditExSeam seAdditionalTime7_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:330</summary>
    public TSpinEditExSeam seAdditionalTime8_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:331</summary>
    public TSpinEditExSeam seAdditionalTime9_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:332</summary>
    public TSpinEditExSeam seAdditionalTime10_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:333</summary>
    public TComboBoxSeam cbbPushedType4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:334</summary>
    public TTabSheetSeam tsSubAttrib = new();
    /// <summary>原文 uFrmCustomMagic.pas:335</summary>
    public TPanelSeam pnlMagicServer = new();
    /// <summary>原文 uFrmCustomMagic.pas:336</summary>
    public TLabelSeam lbl12 = new();
    /// <summary>原文 uFrmCustomMagic.pas:337</summary>
    public TLabelSeam lblAttackDelay = new();
    /// <summary>原文 uFrmCustomMagic.pas:338</summary>
    public TLabelSeam lblAttackDelayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:339</summary>
    public TComboBoxSeam cbbOperateMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:340</summary>
    public TGroupBoxSeam grpInterval = new();
    /// <summary>原文 uFrmCustomMagic.pas:341</summary>
    public TLabelSeam Label32 = new();
    /// <summary>原文 uFrmCustomMagic.pas:342</summary>
    public TLabelSeam Label33 = new();
    /// <summary>原文 uFrmCustomMagic.pas:343</summary>
    public TLabelSeam Label34 = new();
    /// <summary>原文 uFrmCustomMagic.pas:344</summary>
    public TLabelSeam Label35 = new();
    /// <summary>原文 uFrmCustomMagic.pas:345</summary>
    public TSpinEditExSeam seUseInterval = new();
    /// <summary>原文 uFrmCustomMagic.pas:346</summary>
    public TCheckBoxSeam chkFailNoShowEff = new();
    /// <summary>原文 uFrmCustomMagic.pas:347</summary>
    public TEditSeam edtFailMsg = new();
    /// <summary>原文 uFrmCustomMagic.pas:348</summary>
    public TEditSeam edtCloseMsg = new();
    /// <summary>原文 uFrmCustomMagic.pas:349</summary>
    public TEditSeam edtSucceedMsg = new();
    /// <summary>原文 uFrmCustomMagic.pas:350</summary>
    public TSpinEditExSeam seAttackDelayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:351</summary>
    public TCheckBoxSeam chkAttackUseNG = new();
    /// <summary>原文 uFrmCustomMagic.pas:352</summary>
    public TGroupBoxSeam grpNeedItem = new();
    /// <summary>原文 uFrmCustomMagic.pas:353</summary>
    public TLabelSeam lblNeedItem = new();
    /// <summary>原文 uFrmCustomMagic.pas:354</summary>
    public TLabelSeam lblNeedItemCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:355</summary>
    public TLabelSeam lblNeedItemCustomItemName = new();
    /// <summary>原文 uFrmCustomMagic.pas:356</summary>
    public TLabelSeam lblCheckVarName = new();
    /// <summary>原文 uFrmCustomMagic.pas:357</summary>
    public TLabelSeam lblCheckVarType = new();
    /// <summary>原文 uFrmCustomMagic.pas:358</summary>
    public TLabelSeam lblCheckVarValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:359</summary>
    public TLabelSeam lblCheckVarAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:360</summary>
    public TComboBoxSeam cbbNeedItem = new();
    /// <summary>原文 uFrmCustomMagic.pas:361</summary>
    public TSpinEditExSeam seNeedItemCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:362</summary>
    public TEditSeam edtNeedItemCustomItemName = new();
    /// <summary>原文 uFrmCustomMagic.pas:363</summary>
    public TCheckBoxSeam chkNeedItemUseBagItem = new();
    /// <summary>原文 uFrmCustomMagic.pas:364</summary>
    public TCheckBoxSeam chkCheckVarValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:365</summary>
    public TEditSeam edtCheckVarName = new();
    /// <summary>原文 uFrmCustomMagic.pas:366</summary>
    public TComboBoxSeam cbbCheckVarType = new();
    /// <summary>原文 uFrmCustomMagic.pas:367</summary>
    public TSpinEditExSeam seCheckVarValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:368</summary>
    public TSpinEditExSeam seCheckVarAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:369</summary>
    public TLabelSeam lbl14 = new();
    /// <summary>原文 uFrmCustomMagic.pas:370</summary>
    public CustomMagicTreeHost vstAttackDecAttr = new();
    /// <summary>原文 uFrmCustomMagic.pas:371</summary>
    public TImageListSeam ilCheck = new();
    /// <summary>原文 uFrmCustomMagic.pas:372</summary>
    public TTabSheetSeam tsAttackDecElement = new();
    /// <summary>原文 uFrmCustomMagic.pas:373</summary>
    public CustomMagicTreeHost vstDecElement = new();
    /// <summary>原文 uFrmCustomMagic.pas:374</summary>
    public TPageControlSeam pgcProtected = new();
    /// <summary>原文 uFrmCustomMagic.pas:375</summary>
    public TTabSheetSeam tsProtectedDec = new();
    /// <summary>原文 uFrmCustomMagic.pas:376</summary>
    public TTabSheetSeam tsProtectedAddElement = new();
    /// <summary>原文 uFrmCustomMagic.pas:377</summary>
    public TCheckBoxSeam chkProtectAddHPSlow = new();
    /// <summary>原文 uFrmCustomMagic.pas:378</summary>
    public CustomMagicTreeHost vstAddElement = new();
    /// <summary>原文 uFrmCustomMagic.pas:379</summary>
    public TComboBoxSeam cbbAdditionalTime0_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:380</summary>
    public TComboBoxSeam cbbAdditionalTime1_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:381</summary>
    public TComboBoxSeam cbbAdditionalTime2_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:382</summary>
    public TComboBoxSeam cbbAdditionalTime3_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:383</summary>
    public TComboBoxSeam cbbAdditionalTime10_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:384</summary>
    public TComboBoxSeam cbbAdditionalTime7_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:385</summary>
    public TComboBoxSeam cbbAdditionalTime8_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:386</summary>
    public TComboBoxSeam cbbAdditionalTime9_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:387</summary>
    public TSpinEditExSeam seProtectAddHPSlowCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:388</summary>
    public TLabelSeam lblProtectTargetRangeTitle = new();
    /// <summary>原文 uFrmCustomMagic.pas:389</summary>
    public TLabelSeam lblProtectTargetRangeValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:390</summary>
    public TSpinEditExSeam seProtectTargetRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:391</summary>
    public CustomMagicTreeHost vstProtectedAddAttr = new();
    /// <summary>原文 uFrmCustomMagic.pas:392</summary>
    public TTabSheetSeam tsTargetStatus = new();
    /// <summary>原文 uFrmCustomMagic.pas:393</summary>
    public TGroupBoxSeam GroupBox11 = new();
    /// <summary>原文 uFrmCustomMagic.pas:394</summary>
    public TLabelSeam Label177 = new();
    /// <summary>原文 uFrmCustomMagic.pas:395</summary>
    public TLabelSeam Label178 = new();
    /// <summary>原文 uFrmCustomMagic.pas:396</summary>
    public TLabelSeam Label179 = new();
    /// <summary>原文 uFrmCustomMagic.pas:397</summary>
    public TLabelSeam Label180 = new();
    /// <summary>原文 uFrmCustomMagic.pas:398</summary>
    public TLabelSeam Label181 = new();
    /// <summary>原文 uFrmCustomMagic.pas:399</summary>
    public TLabelSeam Label186 = new();
    /// <summary>原文 uFrmCustomMagic.pas:400</summary>
    public TComboBoxSeam cbbTargetStatus1_File = new();
    /// <summary>原文 uFrmCustomMagic.pas:401</summary>
    public TComboBoxSeam cbbTargetStatus1_DrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:402</summary>
    public TSpinEditExSeam seTargetStatus1_PlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:403</summary>
    public TSpinEditExSeam seTargetStatus1_EmptyCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:404</summary>
    public TSpinEditExSeam seTargetStatus1_PlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:405</summary>
    public TSpinEditExSeam seTargetStatus1_StartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:406</summary>
    public TGroupBoxSeam GroupBox13 = new();
    /// <summary>原文 uFrmCustomMagic.pas:407</summary>
    public TLabelSeam Label190 = new();
    /// <summary>原文 uFrmCustomMagic.pas:408</summary>
    public TLabelSeam Label191 = new();
    /// <summary>原文 uFrmCustomMagic.pas:409</summary>
    public TLabelSeam Label192 = new();
    /// <summary>原文 uFrmCustomMagic.pas:410</summary>
    public TLabelSeam Label193 = new();
    /// <summary>原文 uFrmCustomMagic.pas:411</summary>
    public TLabelSeam Label194 = new();
    /// <summary>原文 uFrmCustomMagic.pas:412</summary>
    public TLabelSeam Label196 = new();
    /// <summary>原文 uFrmCustomMagic.pas:413</summary>
    public TComboBoxSeam cbbTargetStatus2_File = new();
    /// <summary>原文 uFrmCustomMagic.pas:414</summary>
    public TComboBoxSeam cbbTargetStatus2_DrawMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:415</summary>
    public TSpinEditExSeam seTargetStatus2_PlayTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:416</summary>
    public TSpinEditExSeam seTargetStatus2_EmptyCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:417</summary>
    public TSpinEditExSeam seTargetStatus2_PlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:418</summary>
    public TSpinEditExSeam seTargetStatus2_StartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:419</summary>
    public TLabelSeam Label201 = new();
    /// <summary>原文 uFrmCustomMagic.pas:420</summary>
    public TLabelSeam Label204 = new();
    /// <summary>原文 uFrmCustomMagic.pas:421</summary>
    public TLabelSeam Label238 = new();
    /// <summary>原文 uFrmCustomMagic.pas:422</summary>
    public TCheckBoxSeam chkAttackTargetStatus = new();
    /// <summary>原文 uFrmCustomMagic.pas:423</summary>
    public TSpinEditExSeam seAttackTargetStatusTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:424</summary>
    public TSpinEditExSeam seAttackTargetStatusTime_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:425</summary>
    public TComboBoxSeam cbbAttackTargetStatusTime_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:426</summary>
    public TLabelSeam Label198 = new();
    /// <summary>原文 uFrmCustomMagic.pas:427</summary>
    public TLabelSeam Label202 = new();
    /// <summary>原文 uFrmCustomMagic.pas:428</summary>
    public TCheckBoxSeam chkProtectTargetStatus = new();
    /// <summary>原文 uFrmCustomMagic.pas:429</summary>
    public TSpinEditExSeam seProtectTargetStatusTime = new();
    /// <summary>原文 uFrmCustomMagic.pas:430</summary>
    public TSpinEditExSeam seProtectTargetStatusTime_2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:431</summary>
    public TLabelSeam lbl7 = new();
    /// <summary>原文 uFrmCustomMagic.pas:432</summary>
    public TLabelSeam Label58 = new();
    /// <summary>原文 uFrmCustomMagic.pas:433</summary>
    public TLabelSeam lbl10 = new();
    /// <summary>原文 uFrmCustomMagic.pas:434</summary>
    public TSpinEditExSeam seProtectTargetStatusTimeDelay = new();
    /// <summary>原文 uFrmCustomMagic.pas:435</summary>
    public TCheckBoxSeam chkTargetStatus1_CalcDir = new();
    /// <summary>原文 uFrmCustomMagic.pas:436</summary>
    public TCheckBoxSeam chkTargetStatus2_CalcDir = new();
    /// <summary>原文 uFrmCustomMagic.pas:437</summary>
    public TLabelSeam Label23 = new();
    /// <summary>原文 uFrmCustomMagic.pas:438</summary>
    public TSpinEditExSeam seAttackTargetStatusDelay = new();
    /// <summary>原文 uFrmCustomMagic.pas:439</summary>
    public TLabelSeam lbl17 = new();
    /// <summary>原文 uFrmCustomMagic.pas:440</summary>
    public TComboBoxSeam cbbProtectTargetStatusTime_1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:441</summary>
    public TLabelSeam Label85 = new();
    /// <summary>原文 uFrmCustomMagic.pas:442</summary>
    public TCheckBoxSeam chkAttackNoChangeDir = new();
    /// <summary>原文 uFrmCustomMagic.pas:443</summary>
    public TLabelSeam Label86 = new();
    /// <summary>原文 uFrmCustomMagic.pas:444</summary>
    public TSpinEditExSeam seClientPreTargetEmptyCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:445</summary>
    public TCheckBoxSeam chkClientPreTargetCalcDir = new();
    /// <summary>原文 uFrmCustomMagic.pas:446</summary>
    public TStaticTextSeam txtMagicWarr = new();
    /// <summary>原文 uFrmCustomMagic.pas:447</summary>
    public TLabelSeam lbl20 = new();
    /// <summary>原文 uFrmCustomMagic.pas:448</summary>
    public TGroupBoxSeam GroupBox3 = new();
    /// <summary>原文 uFrmCustomMagic.pas:449</summary>
    public TLabelSeam lbl4 = new();
    /// <summary>原文 uFrmCustomMagic.pas:450</summary>
    public TLabelSeam Label52 = new();
    /// <summary>原文 uFrmCustomMagic.pas:451</summary>
    public TLabelSeam lbl15 = new();
    /// <summary>原文 uFrmCustomMagic.pas:452</summary>
    public TLabelSeam lbl16 = new();
    /// <summary>原文 uFrmCustomMagic.pas:453</summary>
    public TLabelSeam Label11 = new();
    /// <summary>原文 uFrmCustomMagic.pas:454</summary>
    public TLabelSeam Label195 = new();
    /// <summary>原文 uFrmCustomMagic.pas:455</summary>
    public TLabelSeam lblMagicWarrNGOption = new();
    /// <summary>原文 uFrmCustomMagic.pas:456</summary>
    public TLabelSeam lbl18 = new();
    /// <summary>原文 uFrmCustomMagic.pas:457</summary>
    public TComboBoxSeam cbbClientLevel = new();
    /// <summary>原文 uFrmCustomMagic.pas:458</summary>
    public TCheckBoxSeam chkClientLock = new();
    /// <summary>原文 uFrmCustomMagic.pas:459</summary>
    public TComboBoxSeam cbbClientActionType = new();
    /// <summary>原文 uFrmCustomMagic.pas:460</summary>
    public TCheckBoxSeam chkClientLockSelf = new();
    /// <summary>原文 uFrmCustomMagic.pas:461</summary>
    public TButtonSeam btnCopyConfig = new();
    /// <summary>原文 uFrmCustomMagic.pas:462</summary>
    public TSpinEditExSeam seClientActionStartIndex = new();
    /// <summary>原文 uFrmCustomMagic.pas:463</summary>
    public TSpinEditExSeam seClientActionPlayCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:464</summary>
    public TSpinEditExSeam seClientActionEmptyCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:465</summary>
    public TCheckBoxSeam chkClientActionContinue = new();
    /// <summary>原文 uFrmCustomMagic.pas:466</summary>
    public TComboBoxSeam cbbMagicSwitchMode = new();
    /// <summary>原文 uFrmCustomMagic.pas:467</summary>
    public TComboBoxSeam cbbMagicWarrNGOption = new();
    /// <summary>原文 uFrmCustomMagic.pas:468</summary>
    public TCheckBoxSeam chkSwitchModeNoClose = new();
    /// <summary>原文 uFrmCustomMagic.pas:469</summary>
    public TCheckBoxSeam chkMagicAutoOpen = new();
    /// <summary>原文 uFrmCustomMagic.pas:470</summary>
    public TCheckBoxSeam chkDisableInSafeZone = new();
    /// <summary>原文 uFrmCustomMagic.pas:471</summary>
    public TTabSheetSeam ts1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:472</summary>
    public TLabelSeam Label776 = new();
    /// <summary>原文 uFrmCustomMagic.pas:473</summary>
    public TSpinEditExSeam seMagicACHumValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:474</summary>
    public TSpinEditExSeam seMagicACMonValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:475</summary>
    public TSpinEditExSeam seMagicACHeroValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:476</summary>
    public TSpinEditExSeam seDefenceHumValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:477</summary>
    public TSpinEditExSeam seDefenceMonValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:478</summary>
    public TSpinEditExSeam seDefenceHeroValue = new();
    /// <summary>原文 uFrmCustomMagic.pas:479</summary>
    public TLabelSeam Label92 = new();
    /// <summary>原文 uFrmCustomMagic.pas:480</summary>
    public TLabelSeam Label94 = new();
    /// <summary>原文 uFrmCustomMagic.pas:481</summary>
    public TLabelSeam Label95 = new();
    /// <summary>原文 uFrmCustomMagic.pas:482</summary>
    public TLabelSeam Label97 = new();
    /// <summary>原文 uFrmCustomMagic.pas:483</summary>
    public TLabelSeam Label98 = new();
    /// <summary>原文 uFrmCustomMagic.pas:484</summary>
    public TLabelSeam Label100 = new();
    /// <summary>原文 uFrmCustomMagic.pas:485</summary>
    public TCheckBoxSeam chkMagicACHum = new();
    /// <summary>原文 uFrmCustomMagic.pas:486</summary>
    public TSpinEditExSeam seMagicACHumRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:487</summary>
    public TSpinEditExSeam seMagicACHumRateAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:488</summary>
    public TSpinEditExSeam seMagicACHumValueAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:489</summary>
    public TLabelSeam Label93 = new();
    /// <summary>原文 uFrmCustomMagic.pas:490</summary>
    public TLabelSeam Label137 = new();
    /// <summary>原文 uFrmCustomMagic.pas:491</summary>
    public TLabelSeam Label140 = new();
    /// <summary>原文 uFrmCustomMagic.pas:492</summary>
    public TLabelSeam Label145 = new();
    /// <summary>原文 uFrmCustomMagic.pas:493</summary>
    public TLabelSeam Label146 = new();
    /// <summary>原文 uFrmCustomMagic.pas:494</summary>
    public TLabelSeam Label147 = new();
    /// <summary>原文 uFrmCustomMagic.pas:495</summary>
    public TLabelSeam Label148 = new();
    /// <summary>原文 uFrmCustomMagic.pas:496</summary>
    public TLabelSeam Label149 = new();
    /// <summary>原文 uFrmCustomMagic.pas:497</summary>
    public TCheckBoxSeam chkMagicACMon = new();
    /// <summary>原文 uFrmCustomMagic.pas:498</summary>
    public TSpinEditExSeam seMagicACMonRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:499</summary>
    public TSpinEditExSeam seMagicACMonRateAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:500</summary>
    public TSpinEditExSeam seMagicACMonValueAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:501</summary>
    public TLabelSeam Label150 = new();
    /// <summary>原文 uFrmCustomMagic.pas:502</summary>
    public TLabelSeam Label151 = new();
    /// <summary>原文 uFrmCustomMagic.pas:503</summary>
    public TLabelSeam Label153 = new();
    /// <summary>原文 uFrmCustomMagic.pas:504</summary>
    public TLabelSeam Label155 = new();
    /// <summary>原文 uFrmCustomMagic.pas:505</summary>
    public TLabelSeam Label157 = new();
    /// <summary>原文 uFrmCustomMagic.pas:506</summary>
    public TLabelSeam Label159 = new();
    /// <summary>原文 uFrmCustomMagic.pas:507</summary>
    public TLabelSeam Label160 = new();
    /// <summary>原文 uFrmCustomMagic.pas:508</summary>
    public TLabelSeam Label161 = new();
    /// <summary>原文 uFrmCustomMagic.pas:509</summary>
    public TCheckBoxSeam chkMagicACHero = new();
    /// <summary>原文 uFrmCustomMagic.pas:510</summary>
    public TSpinEditExSeam seMagicACHeroRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:511</summary>
    public TSpinEditExSeam seMagicACHeroRateAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:512</summary>
    public TSpinEditExSeam seMagicACHeroValueAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:513</summary>
    public TLabelSeam Label164 = new();
    /// <summary>原文 uFrmCustomMagic.pas:514</summary>
    public TLabelSeam Label166 = new();
    /// <summary>原文 uFrmCustomMagic.pas:515</summary>
    public TLabelSeam Label167 = new();
    /// <summary>原文 uFrmCustomMagic.pas:516</summary>
    public TLabelSeam Label169 = new();
    /// <summary>原文 uFrmCustomMagic.pas:517</summary>
    public TLabelSeam Label170 = new();
    /// <summary>原文 uFrmCustomMagic.pas:518</summary>
    public TLabelSeam Label171 = new();
    /// <summary>原文 uFrmCustomMagic.pas:519</summary>
    public TLabelSeam Label174 = new();
    /// <summary>原文 uFrmCustomMagic.pas:520</summary>
    public TLabelSeam Label175 = new();
    /// <summary>原文 uFrmCustomMagic.pas:521</summary>
    public TCheckBoxSeam chkDefenceHum = new();
    /// <summary>原文 uFrmCustomMagic.pas:522</summary>
    public TSpinEditExSeam seDefenceHumRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:523</summary>
    public TSpinEditExSeam seDefenceHumRateAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:524</summary>
    public TSpinEditExSeam seDefenceHumValueAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:525</summary>
    public TLabelSeam Label176 = new();
    /// <summary>原文 uFrmCustomMagic.pas:526</summary>
    public TLabelSeam Label182 = new();
    /// <summary>原文 uFrmCustomMagic.pas:527</summary>
    public TLabelSeam Label183 = new();
    /// <summary>原文 uFrmCustomMagic.pas:528</summary>
    public TLabelSeam Label184 = new();
    /// <summary>原文 uFrmCustomMagic.pas:529</summary>
    public TLabelSeam Label185 = new();
    /// <summary>原文 uFrmCustomMagic.pas:530</summary>
    public TLabelSeam Label187 = new();
    /// <summary>原文 uFrmCustomMagic.pas:531</summary>
    public TLabelSeam Label188 = new();
    /// <summary>原文 uFrmCustomMagic.pas:532</summary>
    public TLabelSeam Label189 = new();
    /// <summary>原文 uFrmCustomMagic.pas:533</summary>
    public TCheckBoxSeam chkDefenceMon = new();
    /// <summary>原文 uFrmCustomMagic.pas:534</summary>
    public TSpinEditExSeam seDefenceMonRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:535</summary>
    public TSpinEditExSeam seDefenceMonRateAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:536</summary>
    public TSpinEditExSeam seDefenceMonValueAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:537</summary>
    public TLabelSeam Label197 = new();
    /// <summary>原文 uFrmCustomMagic.pas:538</summary>
    public TLabelSeam Label66 = new();
    /// <summary>原文 uFrmCustomMagic.pas:539</summary>
    public TLabelSeam Label87 = new();
    /// <summary>原文 uFrmCustomMagic.pas:540</summary>
    public TLabelSeam Label88 = new();
    /// <summary>原文 uFrmCustomMagic.pas:541</summary>
    public TLabelSeam Label89 = new();
    /// <summary>原文 uFrmCustomMagic.pas:542</summary>
    public TLabelSeam Label203 = new();
    /// <summary>原文 uFrmCustomMagic.pas:543</summary>
    public TLabelSeam Label239 = new();
    /// <summary>原文 uFrmCustomMagic.pas:544</summary>
    public TLabelSeam Label240 = new();
    /// <summary>原文 uFrmCustomMagic.pas:545</summary>
    public TCheckBoxSeam chkDefenceHero = new();
    /// <summary>原文 uFrmCustomMagic.pas:546</summary>
    public TSpinEditExSeam seDefenceHeroRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:547</summary>
    public TSpinEditExSeam seDefenceHeroRateAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:548</summary>
    public TSpinEditExSeam seDefenceHeroValueAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:549</summary>
    public TLabelSeam Label241 = new();
    /// <summary>原文 uFrmCustomMagic.pas:550</summary>
    public TLabelSeam lbl19 = new();
    /// <summary>原文 uFrmCustomMagic.pas:551</summary>
    public TLabelSeam Label90 = new();
    /// <summary>原文 uFrmCustomMagic.pas:552</summary>
    public TLabelSeam Label91 = new();
    /// <summary>原文 uFrmCustomMagic.pas:553</summary>
    public TCheckBoxSeam chkClientNotRaiseHand = new();
    /// <summary>原文 uFrmCustomMagic.pas:554</summary>
    public TGroupBoxSeam grpOptions = new();
    /// <summary>原文 uFrmCustomMagic.pas:555</summary>
    public TLabelSeam Label6 = new();
    /// <summary>原文 uFrmCustomMagic.pas:556</summary>
    public TLabelSeam Label7 = new();
    /// <summary>原文 uFrmCustomMagic.pas:557</summary>
    public TLabelSeam Label8 = new();
    /// <summary>原文 uFrmCustomMagic.pas:558</summary>
    public TLabelSeam Label9 = new();
    /// <summary>原文 uFrmCustomMagic.pas:559</summary>
    public TLabelSeam Label12 = new();
    /// <summary>原文 uFrmCustomMagic.pas:560</summary>
    public TSpinEditExSeam seAttackNearRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:561</summary>
    public TSpinEditExSeam seAttackGroupRange = new();
    /// <summary>原文 uFrmCustomMagic.pas:562</summary>
    public TComboBoxSeam cbbAttackTarget = new();
    /// <summary>原文 uFrmCustomMagic.pas:563</summary>
    public TCheckBoxSeam chkEnableAntiMagic = new();
    /// <summary>原文 uFrmCustomMagic.pas:564</summary>
    public TCheckBoxSeam chkEnableHitPoint = new();
    /// <summary>原文 uFrmCustomMagic.pas:565</summary>
    public TGroupBoxSeam grpCallMob = new();
    /// <summary>原文 uFrmCustomMagic.pas:566</summary>
    public TLabelSeam lbl11 = new();
    /// <summary>原文 uFrmCustomMagic.pas:567</summary>
    public TLabelSeam Label207 = new();
    /// <summary>原文 uFrmCustomMagic.pas:568</summary>
    public TLabelSeam Label214 = new();
    /// <summary>原文 uFrmCustomMagic.pas:569</summary>
    public TLabelSeam Label215 = new();
    /// <summary>原文 uFrmCustomMagic.pas:570</summary>
    public TLabelSeam Label210 = new();
    /// <summary>原文 uFrmCustomMagic.pas:571</summary>
    public TLabelSeam Label211 = new();
    /// <summary>原文 uFrmCustomMagic.pas:572</summary>
    public TLabelSeam lbl8 = new();
    /// <summary>原文 uFrmCustomMagic.pas:573</summary>
    public TLabelSeam lbl9 = new();
    /// <summary>原文 uFrmCustomMagic.pas:574</summary>
    public TLabelSeam Label199 = new();
    /// <summary>原文 uFrmCustomMagic.pas:575</summary>
    public TEditSeam edtCallMonster1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:576</summary>
    public TSpinEditExSeam seCallMonsterNum1 = new();
    /// <summary>原文 uFrmCustomMagic.pas:577</summary>
    public TCheckBoxSeam chkEnabledCallMonster = new();
    /// <summary>原文 uFrmCustomMagic.pas:578</summary>
    public TSpinEditExSeam seCallMonstersRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:579</summary>
    public TEditSeam edtCallMonster2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:580</summary>
    public TSpinEditExSeam seCallMonsterNum2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:581</summary>
    public TSpinEditExSeam seCallMonstersRoyaltySec = new();
    /// <summary>原文 uFrmCustomMagic.pas:582</summary>
    public TSpinEditExSeam seCallMonstersLevel = new();
    /// <summary>原文 uFrmCustomMagic.pas:583</summary>
    public TGroupBoxSeam grpPower = new();
    /// <summary>原文 uFrmCustomMagic.pas:584</summary>
    public TLabelSeam Label10 = new();
    /// <summary>原文 uFrmCustomMagic.pas:585</summary>
    public TLabelSeam Label13 = new();
    /// <summary>原文 uFrmCustomMagic.pas:586</summary>
    public TLabelSeam lbl2 = new();
    /// <summary>原文 uFrmCustomMagic.pas:587</summary>
    public TLabelSeam Label53 = new();
    /// <summary>原文 uFrmCustomMagic.pas:588</summary>
    public TLabelSeam lblLineAttackAddPower = new();
    /// <summary>原文 uFrmCustomMagic.pas:589</summary>
    public TLabelSeam lblLineAttackAddPowerPerc = new();
    /// <summary>原文 uFrmCustomMagic.pas:590</summary>
    public TLabelSeam Label206 = new();
    /// <summary>原文 uFrmCustomMagic.pas:591</summary>
    public TLabelSeam Label208 = new();
    /// <summary>原文 uFrmCustomMagic.pas:592</summary>
    public TSpinEditExSeam seAttackPowerRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:593</summary>
    public TComboBoxSeam cbbAttackPowerLevel = new();
    /// <summary>原文 uFrmCustomMagic.pas:594</summary>
    public TComboBoxSeam cbbAttackPowerCalc = new();
    /// <summary>原文 uFrmCustomMagic.pas:595</summary>
    public TSpinEditExSeam seLineAttackAddPower = new();
    /// <summary>原文 uFrmCustomMagic.pas:596</summary>
    public TSpinEditExSeam seAttackPowerUndeadAdd = new();
    /// <summary>原文 uFrmCustomMagic.pas:597</summary>
    public TGroupBoxSeam grpMove = new();
    /// <summary>原文 uFrmCustomMagic.pas:598</summary>
    public TLabelSeam Label216 = new();
    /// <summary>原文 uFrmCustomMagic.pas:599</summary>
    public TLabelSeam Label217 = new();
    /// <summary>原文 uFrmCustomMagic.pas:600</summary>
    public TLabelSeam Label242 = new();
    /// <summary>原文 uFrmCustomMagic.pas:601</summary>
    public TSpinEditExSeam seAttackTeleportRate = new();
    /// <summary>原文 uFrmCustomMagic.pas:602</summary>
    public TCheckBoxSeam chkAttackTeleportRunHum = new();
    /// <summary>原文 uFrmCustomMagic.pas:603</summary>
    public TCheckBoxSeam chkAttackTeleportRunMon = new();
    /// <summary>原文 uFrmCustomMagic.pas:604</summary>
    public TCheckBoxSeam chkAttackTeleportRunNpc = new();
    /// <summary>原文 uFrmCustomMagic.pas:605</summary>
    public TCheckBoxSeam chkAttackTeleportRunGuard = new();
    /// <summary>原文 uFrmCustomMagic.pas:606</summary>
    public TCheckBoxSeam chkAttackTeleportRunObstacle = new();
    /// <summary>原文 uFrmCustomMagic.pas:607</summary>
    public TCheckBoxSeam chkAttackTeleportWarDisHumRun = new();
    /// <summary>原文 uFrmCustomMagic.pas:608</summary>
    public TCheckBoxSeam chkAttackTeleportCannotRunItem = new();
    /// <summary>原文 uFrmCustomMagic.pas:609</summary>
    public TCheckBoxSeam chkNoTeleportNoAttack = new();
    /// <summary>原文 uFrmCustomMagic.pas:610</summary>
    public TCheckBoxSeam chkAttackTeleportAfterDamage = new();
    /// <summary>原文 uFrmCustomMagic.pas:611</summary>
    public TCheckBoxSeam chkAttackTeleportRush = new();
    /// <summary>原文 uFrmCustomMagic.pas:612</summary>
    public TSpinEditExSeam seAttackTeleportRushCount = new();
    /// <summary>原文 uFrmCustomMagic.pas:613</summary>
    public TCheckBoxSeam chkAttackTeleportAttack = new();
    /// <summary>原文 uFrmCustomMagic.pas:614</summary>
    public TLabelSeam lblAttackWidth = new();
    /// <summary>原文 uFrmCustomMagic.pas:615</summary>
    public TLabelSeam lblH_AttackWidth = new();
    /// <summary>原文 uFrmCustomMagic.pas:616</summary>
    public TSpinEditExSeam seAttackLineWidth = new();
}
