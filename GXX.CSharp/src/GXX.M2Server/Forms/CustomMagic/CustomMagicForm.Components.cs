// ============================================================================
// uFrmCustomMagic.pas（Source\M2Engine\Forms\uFrmCustomMagic.pas，4800 行，GBK）1:1 移植
// 车道 p5-m2-custommagic ｜ 命名空间 GXX.M2Server.Forms.CustomMagic
//
// 本文件 = TFrmCustomMagic 的**全部 published 组件字段**（原文 :17-616，共 600 个），
// 字段名 1:1；类型由本车道的接缝类型承接（VirtualTrees / SpinEditEx 属未移植第三方，
// 见 CustomMagicSeams.cs 顶部说明）。字段顺序 = 原文声明顺序，行号见每行末尾注释。
//
// 覆盖行号（Delphi）：uFrmCustomMagic.pas 17-616
// ============================================================================

namespace GXX.M2Server.Forms.CustomMagic;

public partial class TFrmCustomMagic
{
    /// <summary>原文 uFrmCustomMagic.pas:17</summary>
    public TGroupBoxSeam grpMonster = null!;
    /// <summary>原文 uFrmCustomMagic.pas:18</summary>
    public CustomMagicTreeHost vstCustomMagic = null!;
    /// <summary>原文 uFrmCustomMagic.pas:19</summary>
    public TPageControlSeam pgcMain = null!;
    /// <summary>原文 uFrmCustomMagic.pas:20</summary>
    public TTabSheetSeam tsAttack = null!;
    /// <summary>原文 uFrmCustomMagic.pas:21</summary>
    public TTabSheetSeam tsServerAttack = null!;
    /// <summary>原文 uFrmCustomMagic.pas:22</summary>
    public TPanelSeam pnlBottom = null!;
    /// <summary>原文 uFrmCustomMagic.pas:23</summary>
    public TLabelSeam lbl13 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:24</summary>
    public TButtonSeam btnSave = null!;
    /// <summary>原文 uFrmCustomMagic.pas:25</summary>
    public TCheckBoxSeam chkSendCustomMagicConfig = null!;
    /// <summary>原文 uFrmCustomMagic.pas:26</summary>
    public TButtonSeam btnMakeConfigData = null!;
    /// <summary>原文 uFrmCustomMagic.pas:27</summary>
    public TSaveDialogSeam dlgSaveMagics = null!;
    /// <summary>原文 uFrmCustomMagic.pas:28</summary>
    public TPageControlSeam pgcClient = null!;
    /// <summary>原文 uFrmCustomMagic.pas:29</summary>
    public TTabSheetSeam tsBase = null!;
    /// <summary>原文 uFrmCustomMagic.pas:30</summary>
    public TGroupBoxSeam grpClientBaseSetting = null!;
    /// <summary>原文 uFrmCustomMagic.pas:31</summary>
    public TLabelSeam Label50 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:32</summary>
    public TLabelSeam Label51 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:33</summary>
    public TComboBoxSeam cbbClientIconFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:34</summary>
    public TSpinEditExSeam seClientIconIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:35</summary>
    public TGroupBoxSeam GroupBox2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:36</summary>
    public TLabelSeam Label225 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:37</summary>
    public TLabelSeam Label226 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:38</summary>
    public TLabelSeam lbl3 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:39</summary>
    public TLabelSeam Label227 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:40</summary>
    public TLabelSeam Label228 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:41</summary>
    public TEditSeam edtSound1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:42</summary>
    public TEditSeam edtSound2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:43</summary>
    public TEditSeam edtSound3 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:44</summary>
    public TEditSeam edtSound4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:45</summary>
    public TEditSeam edtSound5 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:46</summary>
    public TTabSheetSeam tsEffect = null!;
    /// <summary>原文 uFrmCustomMagic.pas:47</summary>
    public TGroupBoxSeam grpFly = null!;
    /// <summary>原文 uFrmCustomMagic.pas:48</summary>
    public TLabelSeam lbl5 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:49</summary>
    public TLabelSeam Label1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:50</summary>
    public TLabelSeam Label2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:51</summary>
    public TLabelSeam Label3 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:52</summary>
    public TLabelSeam Label4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:53</summary>
    public TLabelSeam Label5 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:54</summary>
    public TLabelSeam Label15 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:55</summary>
    public TLabelSeam Label16 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:56</summary>
    public TComboBoxSeam cbbClientFlyFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:57</summary>
    public TComboBoxSeam cbbClientFlyDrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:58</summary>
    public TComboBoxSeam cbbClientFlyDirCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:59</summary>
    public TCheckBoxSeam chkClientFlyCalcDir = null!;
    /// <summary>原文 uFrmCustomMagic.pas:60</summary>
    public TSpinEditExSeam seClientFlyPlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:61</summary>
    public TSpinEditExSeam seClientFlyEmptyCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:62</summary>
    public TSpinEditExSeam seClientFlyPlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:63</summary>
    public TSpinEditExSeam seClientFlyStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:64</summary>
    public TSpinEditExSeam seClientFlyLightRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:65</summary>
    public TGroupBoxSeam grpSelf = null!;
    /// <summary>原文 uFrmCustomMagic.pas:66</summary>
    public TLabelSeam Label17 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:67</summary>
    public TLabelSeam Label18 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:68</summary>
    public TLabelSeam Label19 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:69</summary>
    public TLabelSeam Label20 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:70</summary>
    public TLabelSeam Label21 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:71</summary>
    public TLabelSeam Label22 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:72</summary>
    public TLabelSeam Label24 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:73</summary>
    public TLabelSeam Label25 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:74</summary>
    public TLabelSeam Label96 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:75</summary>
    public TLabelSeam Label99 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:76</summary>
    public TComboBoxSeam cbbClientSelfFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:77</summary>
    public TComboBoxSeam cbbClientSelfDrawOrder = null!;
    /// <summary>原文 uFrmCustomMagic.pas:78</summary>
    public TSpinEditExSeam seClientSelfPlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:79</summary>
    public TSpinEditExSeam seClientSelfPlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:80</summary>
    public TSpinEditExSeam seClientSelfStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:81</summary>
    public TComboBoxSeam cbbClientSelfDrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:82</summary>
    public TSpinEditExSeam seClientSelfEmptyCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:83</summary>
    public TComboBoxSeam cbbClientSelfDirCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:84</summary>
    public TCheckBoxSeam chkClientSelfPlayDelayAction = null!;
    /// <summary>原文 uFrmCustomMagic.pas:85</summary>
    public TSpinEditExSeam seClientSelfLightRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:86</summary>
    public TComboBoxSeam cbbClientSelfDirCalcType = null!;
    /// <summary>原文 uFrmCustomMagic.pas:87</summary>
    public TGroupBoxSeam grpTarget = null!;
    /// <summary>原文 uFrmCustomMagic.pas:88</summary>
    public TLabelSeam Label37 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:89</summary>
    public TLabelSeam Label38 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:90</summary>
    public TLabelSeam Label39 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:91</summary>
    public TLabelSeam Label40 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:92</summary>
    public TLabelSeam Label41 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:93</summary>
    public TLabelSeam Label42 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:94</summary>
    public TLabelSeam Label43 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:95</summary>
    public TLabelSeam Label44 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:96</summary>
    public TLabelSeam Label45 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:97</summary>
    public TLabelSeam Label47 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:98</summary>
    public TBevelSeam bvl1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:99</summary>
    public TComboBoxSeam cbbClientTargetFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:100</summary>
    public TSpinEditExSeam seClientTargetPlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:101</summary>
    public TSpinEditExSeam seClientTargetPlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:102</summary>
    public TSpinEditExSeam seClientTargetStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:103</summary>
    public TComboBoxSeam cbbClientTargetDrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:104</summary>
    public TCheckBoxSeam chkClientTargetMultiPlay = null!;
    /// <summary>原文 uFrmCustomMagic.pas:105</summary>
    public TCheckBoxSeam chkClientTargetLockDraw = null!;
    /// <summary>原文 uFrmCustomMagic.pas:106</summary>
    public TSpinEditExSeam seClientTargetLightRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:107</summary>
    public TCheckBoxSeam chkClientTargetKeepPlay = null!;
    /// <summary>原文 uFrmCustomMagic.pas:108</summary>
    public TSpinEditExSeam seClientTargetKeepTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:109</summary>
    public TSpinEditExSeam seClientTargetKeepAttackRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:110</summary>
    public TSpinEditExSeam seClientTargetKeepAttackInterval = null!;
    /// <summary>原文 uFrmCustomMagic.pas:111</summary>
    public TCheckBoxSeam chkClientTargetKeepMultiPlay = null!;
    /// <summary>原文 uFrmCustomMagic.pas:112</summary>
    public TGroupBoxSeam grpFlyEff = null!;
    /// <summary>原文 uFrmCustomMagic.pas:113</summary>
    public TLabelSeam Label14 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:114</summary>
    public TLabelSeam Label48 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:115</summary>
    public TLabelSeam Label49 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:116</summary>
    public TComboBoxSeam cbbClientFlyEffFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:117</summary>
    public TSpinEditExSeam seClientFlyEffStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:118</summary>
    public TComboBoxSeam cbbClientFlyEffDrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:119</summary>
    public TGroupBoxSeam GroupBox1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:120</summary>
    public TLabelSeam Label26 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:121</summary>
    public TLabelSeam Label27 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:122</summary>
    public TLabelSeam Label28 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:123</summary>
    public TLabelSeam Label29 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:124</summary>
    public TLabelSeam Label30 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:125</summary>
    public TLabelSeam Label36 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:126</summary>
    public TComboBoxSeam cbbClientPreTargetFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:127</summary>
    public TSpinEditExSeam seClientPreTargetPlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:128</summary>
    public TSpinEditExSeam seClientPreTargetPlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:129</summary>
    public TSpinEditExSeam seClientPreTargetStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:130</summary>
    public TComboBoxSeam cbbClientPreTargetDrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:131</summary>
    public TCheckBoxSeam chkClientPreTargetLockDraw = null!;
    /// <summary>原文 uFrmCustomMagic.pas:132</summary>
    public TSpinEditExSeam seClientPreTargetLightRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:133</summary>
    public TGroupBoxSeam grpFastMove = null!;
    /// <summary>原文 uFrmCustomMagic.pas:134</summary>
    public TLabelSeam Label218 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:135</summary>
    public TLabelSeam Label219 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:136</summary>
    public TLabelSeam Label220 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:137</summary>
    public TLabelSeam Label222 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:138</summary>
    public TLabelSeam Label223 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:139</summary>
    public TLabelSeam Label224 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:140</summary>
    public TLabelSeam Label221 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:141</summary>
    public TComboBoxSeam cbbClientFastMoveFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:142</summary>
    public TSpinEditExSeam seClientFastMovePlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:143</summary>
    public TSpinEditExSeam seClientFastMovePlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:144</summary>
    public TSpinEditExSeam seClientFastMoveStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:145</summary>
    public TComboBoxSeam cbbClientFastMoveDrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:146</summary>
    public TSpinEditExSeam seClientFastMoveEmptyCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:147</summary>
    public TCheckBoxSeam chkClientFastMoveCalcDir = null!;
    /// <summary>原文 uFrmCustomMagic.pas:148</summary>
    public TSpinEditExSeam seFastMoveLightRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:149</summary>
    public TGroupBoxSeam GroupBox4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:150</summary>
    public TLabelSeam Label229 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:151</summary>
    public TLabelSeam Label230 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:152</summary>
    public TLabelSeam Label231 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:153</summary>
    public TLabelSeam Label233 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:154</summary>
    public TLabelSeam Label234 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:155</summary>
    public TComboBoxSeam cbbClientSelfKeepFile = null!;
    /// <summary>原文 uFrmCustomMagic.pas:156</summary>
    public TSpinEditExSeam seClientSelfKeepPlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:157</summary>
    public TSpinEditExSeam seClientSelfKeepPlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:158</summary>
    public TSpinEditExSeam seClientSelfKeepStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:159</summary>
    public TComboBoxSeam cbbClientSelfKeepDrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:160</summary>
    public TLabelSeam Label232 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:161</summary>
    public TSpinEditExSeam seClientSelfKeepTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:162</summary>
    public TLabelSeam Label235 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:163</summary>
    public TLabelSeam lbl6 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:164</summary>
    public TLabelSeam Label236 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:165</summary>
    public TSpinEditExSeam seTargetKeepLightRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:166</summary>
    public TLabelSeam Label334 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:167</summary>
    public TLabelSeam Label335 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:168</summary>
    public TSpinEditExSeam seClientTargetKeepTime2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:169</summary>
    public TCheckBoxSeam chkClientFastMoveNoHitAction = null!;
    /// <summary>原文 uFrmCustomMagic.pas:170</summary>
    public TLabelSeam Label336 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:171</summary>
    public TLabelSeam Label337 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:172</summary>
    public TSpinEditExSeam seClientSelfKeepTime2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:173</summary>
    public TCheckBoxSeam chkClientSelfPlayFailNoDraw = null!;
    /// <summary>原文 uFrmCustomMagic.pas:174</summary>
    public TLabelSeam Label205 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:175</summary>
    public TEditSeam edtSound6 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:176</summary>
    public TCheckBoxSeam chkClientFlyFireGunMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:177</summary>
    public TLabelSeam lbl1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:178</summary>
    public TLabelSeam Label31 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:179</summary>
    public TLabelSeam Label209 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:180</summary>
    public TSpinEditExSeam seClientPreTargetStartIndex2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:181</summary>
    public TLabelSeam Label212 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:182</summary>
    public TSpinEditExSeam seClientTargetStartIndex2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:183</summary>
    public TLabelSeam Label213 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:184</summary>
    public TComboBoxSeam cbbClientPreTargetDrawMode2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:185</summary>
    public TLabelSeam Label237 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:186</summary>
    public TComboBoxSeam cbbClientTargetDrawMode2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:187</summary>
    public TLabelSeam Label200 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:188</summary>
    public TLabelSeam Label46 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:189</summary>
    public TCheckBoxSeam chkSelf_SyncHumAction = null!;
    /// <summary>原文 uFrmCustomMagic.pas:190</summary>
    public TPageControlSeam pgcMagicType = null!;
    /// <summary>原文 uFrmCustomMagic.pas:191</summary>
    public TTabSheetSeam tsMagicAttack = null!;
    /// <summary>原文 uFrmCustomMagic.pas:192</summary>
    public TTabSheetSeam tsMagicProtected = null!;
    /// <summary>原文 uFrmCustomMagic.pas:193</summary>
    public TPageControlSeam pgcAttack = null!;
    /// <summary>原文 uFrmCustomMagic.pas:194</summary>
    public TTabSheetSeam tsAdditionals = null!;
    /// <summary>原文 uFrmCustomMagic.pas:195</summary>
    public TLabelSeam Label60 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:196</summary>
    public TLabelSeam Label62 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:197</summary>
    public TLabelSeam Label63 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:198</summary>
    public TLabelSeam Label64 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:199</summary>
    public TLabelSeam Label65 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:200</summary>
    public TLabelSeam Label67 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:201</summary>
    public TLabelSeam Label68 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:202</summary>
    public TLabelSeam Label69 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:203</summary>
    public TLabelSeam Label70 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:204</summary>
    public TLabelSeam Label71 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:205</summary>
    public TLabelSeam Label72 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:206</summary>
    public TLabelSeam Label73 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:207</summary>
    public TLabelSeam Label74 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:208</summary>
    public TLabelSeam Label75 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:209</summary>
    public TLabelSeam Label76 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:210</summary>
    public TLabelSeam Label77 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:211</summary>
    public TLabelSeam Label78 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:212</summary>
    public TLabelSeam Label79 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:213</summary>
    public TLabelSeam Label80 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:214</summary>
    public TLabelSeam Label81 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:215</summary>
    public TLabelSeam Label82 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:216</summary>
    public TLabelSeam Label83 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:217</summary>
    public TLabelSeam Label84 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:218</summary>
    public TLabelSeam Label54 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:219</summary>
    public TLabelSeam Label55 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:220</summary>
    public TLabelSeam Label56 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:221</summary>
    public TLabelSeam Label57 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:222</summary>
    public TLabelSeam Label59 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:223</summary>
    public TLabelSeam Label61 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:224</summary>
    public TLabelSeam Label101 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:225</summary>
    public TLabelSeam Label102 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:226</summary>
    public TLabelSeam Label103 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:227</summary>
    public TLabelSeam Label104 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:228</summary>
    public TLabelSeam Label105 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:229</summary>
    public TLabelSeam Label106 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:230</summary>
    public TLabelSeam Label107 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:231</summary>
    public TLabelSeam Label108 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:232</summary>
    public TLabelSeam Label109 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:233</summary>
    public TLabelSeam Label110 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:234</summary>
    public TLabelSeam Label111 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:235</summary>
    public TLabelSeam Label112 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:236</summary>
    public TLabelSeam Label113 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:237</summary>
    public TLabelSeam Label114 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:238</summary>
    public TLabelSeam Label115 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:239</summary>
    public TLabelSeam Label116 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:240</summary>
    public TLabelSeam Label117 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:241</summary>
    public TLabelSeam Label118 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:242</summary>
    public TLabelSeam Label119 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:243</summary>
    public TLabelSeam Label120 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:244</summary>
    public TLabelSeam Label121 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:245</summary>
    public TLabelSeam Label122 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:246</summary>
    public TLabelSeam Label123 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:247</summary>
    public TLabelSeam Label124 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:248</summary>
    public TLabelSeam Label125 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:249</summary>
    public TLabelSeam Label126 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:250</summary>
    public TLabelSeam Label127 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:251</summary>
    public TLabelSeam Label128 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:252</summary>
    public TLabelSeam Label129 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:253</summary>
    public TLabelSeam Label130 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:254</summary>
    public TLabelSeam Label131 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:255</summary>
    public TLabelSeam Label132 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:256</summary>
    public TLabelSeam Label133 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:257</summary>
    public TLabelSeam Label134 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:258</summary>
    public TLabelSeam Label135 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:259</summary>
    public TLabelSeam Label136 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:260</summary>
    public TLabelSeam Label138 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:261</summary>
    public TLabelSeam Label139 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:262</summary>
    public TLabelSeam Label141 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:263</summary>
    public TLabelSeam Label142 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:264</summary>
    public TLabelSeam Label143 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:265</summary>
    public TLabelSeam Label144 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:266</summary>
    public TLabelSeam Label152 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:267</summary>
    public TLabelSeam Label154 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:268</summary>
    public TLabelSeam Label156 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:269</summary>
    public TLabelSeam Label158 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:270</summary>
    public TLabelSeam Label162 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:271</summary>
    public TLabelSeam Label163 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:272</summary>
    public TLabelSeam Label165 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:273</summary>
    public TLabelSeam Label168 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:274</summary>
    public TLabelSeam Label172 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:275</summary>
    public TLabelSeam Label173 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:276</summary>
    public TCheckBoxSeam chkAdditional0 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:277</summary>
    public TSpinEditExSeam seAdditionalRate0 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:278</summary>
    public TSpinEditExSeam seAdditionalTime0 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:279</summary>
    public TCheckBoxSeam chkAdditional1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:280</summary>
    public TSpinEditExSeam seAdditionalRate1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:281</summary>
    public TSpinEditExSeam seAdditionalTime1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:282</summary>
    public TCheckBoxSeam chkAdditional2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:283</summary>
    public TSpinEditExSeam seAdditionalRate2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:284</summary>
    public TSpinEditExSeam seAdditionalTime2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:285</summary>
    public TCheckBoxSeam chkAdditional3 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:286</summary>
    public TSpinEditExSeam seAdditionalRate3 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:287</summary>
    public TSpinEditExSeam seAdditionalTime3 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:288</summary>
    public TCheckBoxSeam chkAdditional4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:289</summary>
    public TSpinEditExSeam seAdditionalRate4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:290</summary>
    public TSpinEditExSeam seAdditionalTime4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:291</summary>
    public TCheckBoxSeam chkAdditional5 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:292</summary>
    public TSpinEditExSeam seAdditionalRate5 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:293</summary>
    public TSpinEditExSeam seAdditionalTime5 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:294</summary>
    public TCheckBoxSeam chkAdditional6 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:295</summary>
    public TSpinEditExSeam seAdditionalRate6 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:296</summary>
    public TSpinEditExSeam seAdditionalTime6 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:297</summary>
    public TCheckBoxSeam chkAdditional7 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:298</summary>
    public TSpinEditExSeam seAdditionalRate7 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:299</summary>
    public TSpinEditExSeam seAdditionalTime7 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:300</summary>
    public TCheckBoxSeam chkAdditional8 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:301</summary>
    public TSpinEditExSeam seAdditionalRate8 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:302</summary>
    public TSpinEditExSeam seAdditionalTime8 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:303</summary>
    public TCheckBoxSeam chkAdditional9 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:304</summary>
    public TSpinEditExSeam seAdditionalRate9 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:305</summary>
    public TSpinEditExSeam seAdditionalTime9 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:306</summary>
    public TCheckBoxSeam chkseAdditionaHighLevel4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:307</summary>
    public TSpinEditExSeam seAdditionaHP0 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:308</summary>
    public TCheckBoxSeam chkAdditional10 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:309</summary>
    public TSpinEditExSeam seAdditionalRate10 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:310</summary>
    public TSpinEditExSeam seAdditionalTime10 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:311</summary>
    public TSpinEditExSeam seAdditionalRate0_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:312</summary>
    public TSpinEditExSeam seAdditionalRate1_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:313</summary>
    public TSpinEditExSeam seAdditionalRate2_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:314</summary>
    public TSpinEditExSeam seAdditionalRate3_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:315</summary>
    public TSpinEditExSeam seAdditionalRate4_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:316</summary>
    public TSpinEditExSeam seAdditionalRate5_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:317</summary>
    public TSpinEditExSeam seAdditionalRate6_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:318</summary>
    public TSpinEditExSeam seAdditionalRate7_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:319</summary>
    public TSpinEditExSeam seAdditionalRate8_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:320</summary>
    public TSpinEditExSeam seAdditionalRate9_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:321</summary>
    public TSpinEditExSeam seAdditionalRate10_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:322</summary>
    public TSpinEditExSeam seAdditionalTime0_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:323</summary>
    public TSpinEditExSeam seAdditionalTime1_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:324</summary>
    public TSpinEditExSeam seAdditionalTime2_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:325</summary>
    public TSpinEditExSeam seAdditionalTime3_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:326</summary>
    public TSpinEditExSeam seAdditionalTime4_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:327</summary>
    public TSpinEditExSeam seAdditionalTime5_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:328</summary>
    public TSpinEditExSeam seAdditionalTime6_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:329</summary>
    public TSpinEditExSeam seAdditionalTime7_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:330</summary>
    public TSpinEditExSeam seAdditionalTime8_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:331</summary>
    public TSpinEditExSeam seAdditionalTime9_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:332</summary>
    public TSpinEditExSeam seAdditionalTime10_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:333</summary>
    public TComboBoxSeam cbbPushedType4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:334</summary>
    public TTabSheetSeam tsSubAttrib = null!;
    /// <summary>原文 uFrmCustomMagic.pas:335</summary>
    public TPanelSeam pnlMagicServer = null!;
    /// <summary>原文 uFrmCustomMagic.pas:336</summary>
    public TLabelSeam lbl12 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:337</summary>
    public TLabelSeam lblAttackDelay = null!;
    /// <summary>原文 uFrmCustomMagic.pas:338</summary>
    public TLabelSeam lblAttackDelayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:339</summary>
    public TComboBoxSeam cbbOperateMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:340</summary>
    public TGroupBoxSeam grpInterval = null!;
    /// <summary>原文 uFrmCustomMagic.pas:341</summary>
    public TLabelSeam Label32 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:342</summary>
    public TLabelSeam Label33 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:343</summary>
    public TLabelSeam Label34 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:344</summary>
    public TLabelSeam Label35 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:345</summary>
    public TSpinEditExSeam seUseInterval = null!;
    /// <summary>原文 uFrmCustomMagic.pas:346</summary>
    public TCheckBoxSeam chkFailNoShowEff = null!;
    /// <summary>原文 uFrmCustomMagic.pas:347</summary>
    public TEditSeam edtFailMsg = null!;
    /// <summary>原文 uFrmCustomMagic.pas:348</summary>
    public TEditSeam edtCloseMsg = null!;
    /// <summary>原文 uFrmCustomMagic.pas:349</summary>
    public TEditSeam edtSucceedMsg = null!;
    /// <summary>原文 uFrmCustomMagic.pas:350</summary>
    public TSpinEditExSeam seAttackDelayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:351</summary>
    public TCheckBoxSeam chkAttackUseNG = null!;
    /// <summary>原文 uFrmCustomMagic.pas:352</summary>
    public TGroupBoxSeam grpNeedItem = null!;
    /// <summary>原文 uFrmCustomMagic.pas:353</summary>
    public TLabelSeam lblNeedItem = null!;
    /// <summary>原文 uFrmCustomMagic.pas:354</summary>
    public TLabelSeam lblNeedItemCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:355</summary>
    public TLabelSeam lblNeedItemCustomItemName = null!;
    /// <summary>原文 uFrmCustomMagic.pas:356</summary>
    public TLabelSeam lblCheckVarName = null!;
    /// <summary>原文 uFrmCustomMagic.pas:357</summary>
    public TLabelSeam lblCheckVarType = null!;
    /// <summary>原文 uFrmCustomMagic.pas:358</summary>
    public TLabelSeam lblCheckVarValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:359</summary>
    public TLabelSeam lblCheckVarAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:360</summary>
    public TComboBoxSeam cbbNeedItem = null!;
    /// <summary>原文 uFrmCustomMagic.pas:361</summary>
    public TSpinEditExSeam seNeedItemCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:362</summary>
    public TEditSeam edtNeedItemCustomItemName = null!;
    /// <summary>原文 uFrmCustomMagic.pas:363</summary>
    public TCheckBoxSeam chkNeedItemUseBagItem = null!;
    /// <summary>原文 uFrmCustomMagic.pas:364</summary>
    public TCheckBoxSeam chkCheckVarValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:365</summary>
    public TEditSeam edtCheckVarName = null!;
    /// <summary>原文 uFrmCustomMagic.pas:366</summary>
    public TComboBoxSeam cbbCheckVarType = null!;
    /// <summary>原文 uFrmCustomMagic.pas:367</summary>
    public TSpinEditExSeam seCheckVarValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:368</summary>
    public TSpinEditExSeam seCheckVarAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:369</summary>
    public TLabelSeam lbl14 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:370</summary>
    public CustomMagicTreeHost vstAttackDecAttr = null!;
    /// <summary>原文 uFrmCustomMagic.pas:371</summary>
    public TImageListSeam ilCheck = null!;
    /// <summary>原文 uFrmCustomMagic.pas:372</summary>
    public TTabSheetSeam tsAttackDecElement = null!;
    /// <summary>原文 uFrmCustomMagic.pas:373</summary>
    public CustomMagicTreeHost vstDecElement = null!;
    /// <summary>原文 uFrmCustomMagic.pas:374</summary>
    public TPageControlSeam pgcProtected = null!;
    /// <summary>原文 uFrmCustomMagic.pas:375</summary>
    public TTabSheetSeam tsProtectedDec = null!;
    /// <summary>原文 uFrmCustomMagic.pas:376</summary>
    public TTabSheetSeam tsProtectedAddElement = null!;
    /// <summary>原文 uFrmCustomMagic.pas:377</summary>
    public TCheckBoxSeam chkProtectAddHPSlow = null!;
    /// <summary>原文 uFrmCustomMagic.pas:378</summary>
    public CustomMagicTreeHost vstAddElement = null!;
    /// <summary>原文 uFrmCustomMagic.pas:379</summary>
    public TComboBoxSeam cbbAdditionalTime0_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:380</summary>
    public TComboBoxSeam cbbAdditionalTime1_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:381</summary>
    public TComboBoxSeam cbbAdditionalTime2_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:382</summary>
    public TComboBoxSeam cbbAdditionalTime3_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:383</summary>
    public TComboBoxSeam cbbAdditionalTime10_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:384</summary>
    public TComboBoxSeam cbbAdditionalTime7_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:385</summary>
    public TComboBoxSeam cbbAdditionalTime8_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:386</summary>
    public TComboBoxSeam cbbAdditionalTime9_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:387</summary>
    public TSpinEditExSeam seProtectAddHPSlowCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:388</summary>
    public TLabelSeam lblProtectTargetRangeTitle = null!;
    /// <summary>原文 uFrmCustomMagic.pas:389</summary>
    public TLabelSeam lblProtectTargetRangeValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:390</summary>
    public TSpinEditExSeam seProtectTargetRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:391</summary>
    public CustomMagicTreeHost vstProtectedAddAttr = null!;
    /// <summary>原文 uFrmCustomMagic.pas:392</summary>
    public TTabSheetSeam tsTargetStatus = null!;
    /// <summary>原文 uFrmCustomMagic.pas:393</summary>
    public TGroupBoxSeam GroupBox11 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:394</summary>
    public TLabelSeam Label177 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:395</summary>
    public TLabelSeam Label178 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:396</summary>
    public TLabelSeam Label179 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:397</summary>
    public TLabelSeam Label180 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:398</summary>
    public TLabelSeam Label181 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:399</summary>
    public TLabelSeam Label186 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:400</summary>
    public TComboBoxSeam cbbTargetStatus1_File = null!;
    /// <summary>原文 uFrmCustomMagic.pas:401</summary>
    public TComboBoxSeam cbbTargetStatus1_DrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:402</summary>
    public TSpinEditExSeam seTargetStatus1_PlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:403</summary>
    public TSpinEditExSeam seTargetStatus1_EmptyCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:404</summary>
    public TSpinEditExSeam seTargetStatus1_PlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:405</summary>
    public TSpinEditExSeam seTargetStatus1_StartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:406</summary>
    public TGroupBoxSeam GroupBox13 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:407</summary>
    public TLabelSeam Label190 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:408</summary>
    public TLabelSeam Label191 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:409</summary>
    public TLabelSeam Label192 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:410</summary>
    public TLabelSeam Label193 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:411</summary>
    public TLabelSeam Label194 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:412</summary>
    public TLabelSeam Label196 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:413</summary>
    public TComboBoxSeam cbbTargetStatus2_File = null!;
    /// <summary>原文 uFrmCustomMagic.pas:414</summary>
    public TComboBoxSeam cbbTargetStatus2_DrawMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:415</summary>
    public TSpinEditExSeam seTargetStatus2_PlayTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:416</summary>
    public TSpinEditExSeam seTargetStatus2_EmptyCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:417</summary>
    public TSpinEditExSeam seTargetStatus2_PlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:418</summary>
    public TSpinEditExSeam seTargetStatus2_StartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:419</summary>
    public TLabelSeam Label201 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:420</summary>
    public TLabelSeam Label204 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:421</summary>
    public TLabelSeam Label238 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:422</summary>
    public TCheckBoxSeam chkAttackTargetStatus = null!;
    /// <summary>原文 uFrmCustomMagic.pas:423</summary>
    public TSpinEditExSeam seAttackTargetStatusTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:424</summary>
    public TSpinEditExSeam seAttackTargetStatusTime_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:425</summary>
    public TComboBoxSeam cbbAttackTargetStatusTime_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:426</summary>
    public TLabelSeam Label198 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:427</summary>
    public TLabelSeam Label202 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:428</summary>
    public TCheckBoxSeam chkProtectTargetStatus = null!;
    /// <summary>原文 uFrmCustomMagic.pas:429</summary>
    public TSpinEditExSeam seProtectTargetStatusTime = null!;
    /// <summary>原文 uFrmCustomMagic.pas:430</summary>
    public TSpinEditExSeam seProtectTargetStatusTime_2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:431</summary>
    public TLabelSeam lbl7 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:432</summary>
    public TLabelSeam Label58 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:433</summary>
    public TLabelSeam lbl10 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:434</summary>
    public TSpinEditExSeam seProtectTargetStatusTimeDelay = null!;
    /// <summary>原文 uFrmCustomMagic.pas:435</summary>
    public TCheckBoxSeam chkTargetStatus1_CalcDir = null!;
    /// <summary>原文 uFrmCustomMagic.pas:436</summary>
    public TCheckBoxSeam chkTargetStatus2_CalcDir = null!;
    /// <summary>原文 uFrmCustomMagic.pas:437</summary>
    public TLabelSeam Label23 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:438</summary>
    public TSpinEditExSeam seAttackTargetStatusDelay = null!;
    /// <summary>原文 uFrmCustomMagic.pas:439</summary>
    public TLabelSeam lbl17 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:440</summary>
    public TComboBoxSeam cbbProtectTargetStatusTime_1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:441</summary>
    public TLabelSeam Label85 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:442</summary>
    public TCheckBoxSeam chkAttackNoChangeDir = null!;
    /// <summary>原文 uFrmCustomMagic.pas:443</summary>
    public TLabelSeam Label86 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:444</summary>
    public TSpinEditExSeam seClientPreTargetEmptyCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:445</summary>
    public TCheckBoxSeam chkClientPreTargetCalcDir = null!;
    /// <summary>原文 uFrmCustomMagic.pas:446</summary>
    public TStaticTextSeam txtMagicWarr = null!;
    /// <summary>原文 uFrmCustomMagic.pas:447</summary>
    public TLabelSeam lbl20 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:448</summary>
    public TGroupBoxSeam GroupBox3 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:449</summary>
    public TLabelSeam lbl4 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:450</summary>
    public TLabelSeam Label52 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:451</summary>
    public TLabelSeam lbl15 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:452</summary>
    public TLabelSeam lbl16 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:453</summary>
    public TLabelSeam Label11 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:454</summary>
    public TLabelSeam Label195 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:455</summary>
    public TLabelSeam lblMagicWarrNGOption = null!;
    /// <summary>原文 uFrmCustomMagic.pas:456</summary>
    public TLabelSeam lbl18 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:457</summary>
    public TComboBoxSeam cbbClientLevel = null!;
    /// <summary>原文 uFrmCustomMagic.pas:458</summary>
    public TCheckBoxSeam chkClientLock = null!;
    /// <summary>原文 uFrmCustomMagic.pas:459</summary>
    public TComboBoxSeam cbbClientActionType = null!;
    /// <summary>原文 uFrmCustomMagic.pas:460</summary>
    public TCheckBoxSeam chkClientLockSelf = null!;
    /// <summary>原文 uFrmCustomMagic.pas:461</summary>
    public TButtonSeam btnCopyConfig = null!;
    /// <summary>原文 uFrmCustomMagic.pas:462</summary>
    public TSpinEditExSeam seClientActionStartIndex = null!;
    /// <summary>原文 uFrmCustomMagic.pas:463</summary>
    public TSpinEditExSeam seClientActionPlayCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:464</summary>
    public TSpinEditExSeam seClientActionEmptyCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:465</summary>
    public TCheckBoxSeam chkClientActionContinue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:466</summary>
    public TComboBoxSeam cbbMagicSwitchMode = null!;
    /// <summary>原文 uFrmCustomMagic.pas:467</summary>
    public TComboBoxSeam cbbMagicWarrNGOption = null!;
    /// <summary>原文 uFrmCustomMagic.pas:468</summary>
    public TCheckBoxSeam chkSwitchModeNoClose = null!;
    /// <summary>原文 uFrmCustomMagic.pas:469</summary>
    public TCheckBoxSeam chkMagicAutoOpen = null!;
    /// <summary>原文 uFrmCustomMagic.pas:470</summary>
    public TCheckBoxSeam chkDisableInSafeZone = null!;
    /// <summary>原文 uFrmCustomMagic.pas:471</summary>
    public TTabSheetSeam ts1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:472</summary>
    public TLabelSeam Label776 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:473</summary>
    public TSpinEditExSeam seMagicACHumValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:474</summary>
    public TSpinEditExSeam seMagicACMonValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:475</summary>
    public TSpinEditExSeam seMagicACHeroValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:476</summary>
    public TSpinEditExSeam seDefenceHumValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:477</summary>
    public TSpinEditExSeam seDefenceMonValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:478</summary>
    public TSpinEditExSeam seDefenceHeroValue = null!;
    /// <summary>原文 uFrmCustomMagic.pas:479</summary>
    public TLabelSeam Label92 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:480</summary>
    public TLabelSeam Label94 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:481</summary>
    public TLabelSeam Label95 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:482</summary>
    public TLabelSeam Label97 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:483</summary>
    public TLabelSeam Label98 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:484</summary>
    public TLabelSeam Label100 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:485</summary>
    public TCheckBoxSeam chkMagicACHum = null!;
    /// <summary>原文 uFrmCustomMagic.pas:486</summary>
    public TSpinEditExSeam seMagicACHumRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:487</summary>
    public TSpinEditExSeam seMagicACHumRateAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:488</summary>
    public TSpinEditExSeam seMagicACHumValueAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:489</summary>
    public TLabelSeam Label93 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:490</summary>
    public TLabelSeam Label137 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:491</summary>
    public TLabelSeam Label140 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:492</summary>
    public TLabelSeam Label145 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:493</summary>
    public TLabelSeam Label146 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:494</summary>
    public TLabelSeam Label147 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:495</summary>
    public TLabelSeam Label148 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:496</summary>
    public TLabelSeam Label149 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:497</summary>
    public TCheckBoxSeam chkMagicACMon = null!;
    /// <summary>原文 uFrmCustomMagic.pas:498</summary>
    public TSpinEditExSeam seMagicACMonRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:499</summary>
    public TSpinEditExSeam seMagicACMonRateAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:500</summary>
    public TSpinEditExSeam seMagicACMonValueAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:501</summary>
    public TLabelSeam Label150 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:502</summary>
    public TLabelSeam Label151 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:503</summary>
    public TLabelSeam Label153 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:504</summary>
    public TLabelSeam Label155 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:505</summary>
    public TLabelSeam Label157 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:506</summary>
    public TLabelSeam Label159 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:507</summary>
    public TLabelSeam Label160 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:508</summary>
    public TLabelSeam Label161 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:509</summary>
    public TCheckBoxSeam chkMagicACHero = null!;
    /// <summary>原文 uFrmCustomMagic.pas:510</summary>
    public TSpinEditExSeam seMagicACHeroRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:511</summary>
    public TSpinEditExSeam seMagicACHeroRateAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:512</summary>
    public TSpinEditExSeam seMagicACHeroValueAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:513</summary>
    public TLabelSeam Label164 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:514</summary>
    public TLabelSeam Label166 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:515</summary>
    public TLabelSeam Label167 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:516</summary>
    public TLabelSeam Label169 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:517</summary>
    public TLabelSeam Label170 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:518</summary>
    public TLabelSeam Label171 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:519</summary>
    public TLabelSeam Label174 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:520</summary>
    public TLabelSeam Label175 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:521</summary>
    public TCheckBoxSeam chkDefenceHum = null!;
    /// <summary>原文 uFrmCustomMagic.pas:522</summary>
    public TSpinEditExSeam seDefenceHumRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:523</summary>
    public TSpinEditExSeam seDefenceHumRateAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:524</summary>
    public TSpinEditExSeam seDefenceHumValueAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:525</summary>
    public TLabelSeam Label176 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:526</summary>
    public TLabelSeam Label182 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:527</summary>
    public TLabelSeam Label183 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:528</summary>
    public TLabelSeam Label184 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:529</summary>
    public TLabelSeam Label185 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:530</summary>
    public TLabelSeam Label187 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:531</summary>
    public TLabelSeam Label188 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:532</summary>
    public TLabelSeam Label189 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:533</summary>
    public TCheckBoxSeam chkDefenceMon = null!;
    /// <summary>原文 uFrmCustomMagic.pas:534</summary>
    public TSpinEditExSeam seDefenceMonRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:535</summary>
    public TSpinEditExSeam seDefenceMonRateAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:536</summary>
    public TSpinEditExSeam seDefenceMonValueAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:537</summary>
    public TLabelSeam Label197 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:538</summary>
    public TLabelSeam Label66 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:539</summary>
    public TLabelSeam Label87 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:540</summary>
    public TLabelSeam Label88 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:541</summary>
    public TLabelSeam Label89 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:542</summary>
    public TLabelSeam Label203 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:543</summary>
    public TLabelSeam Label239 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:544</summary>
    public TLabelSeam Label240 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:545</summary>
    public TCheckBoxSeam chkDefenceHero = null!;
    /// <summary>原文 uFrmCustomMagic.pas:546</summary>
    public TSpinEditExSeam seDefenceHeroRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:547</summary>
    public TSpinEditExSeam seDefenceHeroRateAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:548</summary>
    public TSpinEditExSeam seDefenceHeroValueAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:549</summary>
    public TLabelSeam Label241 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:550</summary>
    public TLabelSeam lbl19 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:551</summary>
    public TLabelSeam Label90 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:552</summary>
    public TLabelSeam Label91 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:553</summary>
    public TCheckBoxSeam chkClientNotRaiseHand = null!;
    /// <summary>原文 uFrmCustomMagic.pas:554</summary>
    public TGroupBoxSeam grpOptions = null!;
    /// <summary>原文 uFrmCustomMagic.pas:555</summary>
    public TLabelSeam Label6 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:556</summary>
    public TLabelSeam Label7 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:557</summary>
    public TLabelSeam Label8 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:558</summary>
    public TLabelSeam Label9 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:559</summary>
    public TLabelSeam Label12 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:560</summary>
    public TSpinEditExSeam seAttackNearRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:561</summary>
    public TSpinEditExSeam seAttackGroupRange = null!;
    /// <summary>原文 uFrmCustomMagic.pas:562</summary>
    public TComboBoxSeam cbbAttackTarget = null!;
    /// <summary>原文 uFrmCustomMagic.pas:563</summary>
    public TCheckBoxSeam chkEnableAntiMagic = null!;
    /// <summary>原文 uFrmCustomMagic.pas:564</summary>
    public TCheckBoxSeam chkEnableHitPoint = null!;
    /// <summary>原文 uFrmCustomMagic.pas:565</summary>
    public TGroupBoxSeam grpCallMob = null!;
    /// <summary>原文 uFrmCustomMagic.pas:566</summary>
    public TLabelSeam lbl11 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:567</summary>
    public TLabelSeam Label207 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:568</summary>
    public TLabelSeam Label214 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:569</summary>
    public TLabelSeam Label215 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:570</summary>
    public TLabelSeam Label210 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:571</summary>
    public TLabelSeam Label211 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:572</summary>
    public TLabelSeam lbl8 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:573</summary>
    public TLabelSeam lbl9 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:574</summary>
    public TLabelSeam Label199 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:575</summary>
    public TEditSeam edtCallMonster1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:576</summary>
    public TSpinEditExSeam seCallMonsterNum1 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:577</summary>
    public TCheckBoxSeam chkEnabledCallMonster = null!;
    /// <summary>原文 uFrmCustomMagic.pas:578</summary>
    public TSpinEditExSeam seCallMonstersRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:579</summary>
    public TEditSeam edtCallMonster2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:580</summary>
    public TSpinEditExSeam seCallMonsterNum2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:581</summary>
    public TSpinEditExSeam seCallMonstersRoyaltySec = null!;
    /// <summary>原文 uFrmCustomMagic.pas:582</summary>
    public TSpinEditExSeam seCallMonstersLevel = null!;
    /// <summary>原文 uFrmCustomMagic.pas:583</summary>
    public TGroupBoxSeam grpPower = null!;
    /// <summary>原文 uFrmCustomMagic.pas:584</summary>
    public TLabelSeam Label10 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:585</summary>
    public TLabelSeam Label13 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:586</summary>
    public TLabelSeam lbl2 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:587</summary>
    public TLabelSeam Label53 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:588</summary>
    public TLabelSeam lblLineAttackAddPower = null!;
    /// <summary>原文 uFrmCustomMagic.pas:589</summary>
    public TLabelSeam lblLineAttackAddPowerPerc = null!;
    /// <summary>原文 uFrmCustomMagic.pas:590</summary>
    public TLabelSeam Label206 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:591</summary>
    public TLabelSeam Label208 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:592</summary>
    public TSpinEditExSeam seAttackPowerRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:593</summary>
    public TComboBoxSeam cbbAttackPowerLevel = null!;
    /// <summary>原文 uFrmCustomMagic.pas:594</summary>
    public TComboBoxSeam cbbAttackPowerCalc = null!;
    /// <summary>原文 uFrmCustomMagic.pas:595</summary>
    public TSpinEditExSeam seLineAttackAddPower = null!;
    /// <summary>原文 uFrmCustomMagic.pas:596</summary>
    public TSpinEditExSeam seAttackPowerUndeadAdd = null!;
    /// <summary>原文 uFrmCustomMagic.pas:597</summary>
    public TGroupBoxSeam grpMove = null!;
    /// <summary>原文 uFrmCustomMagic.pas:598</summary>
    public TLabelSeam Label216 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:599</summary>
    public TLabelSeam Label217 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:600</summary>
    public TLabelSeam Label242 = null!;
    /// <summary>原文 uFrmCustomMagic.pas:601</summary>
    public TSpinEditExSeam seAttackTeleportRate = null!;
    /// <summary>原文 uFrmCustomMagic.pas:602</summary>
    public TCheckBoxSeam chkAttackTeleportRunHum = null!;
    /// <summary>原文 uFrmCustomMagic.pas:603</summary>
    public TCheckBoxSeam chkAttackTeleportRunMon = null!;
    /// <summary>原文 uFrmCustomMagic.pas:604</summary>
    public TCheckBoxSeam chkAttackTeleportRunNpc = null!;
    /// <summary>原文 uFrmCustomMagic.pas:605</summary>
    public TCheckBoxSeam chkAttackTeleportRunGuard = null!;
    /// <summary>原文 uFrmCustomMagic.pas:606</summary>
    public TCheckBoxSeam chkAttackTeleportRunObstacle = null!;
    /// <summary>原文 uFrmCustomMagic.pas:607</summary>
    public TCheckBoxSeam chkAttackTeleportWarDisHumRun = null!;
    /// <summary>原文 uFrmCustomMagic.pas:608</summary>
    public TCheckBoxSeam chkAttackTeleportCannotRunItem = null!;
    /// <summary>原文 uFrmCustomMagic.pas:609</summary>
    public TCheckBoxSeam chkNoTeleportNoAttack = null!;
    /// <summary>原文 uFrmCustomMagic.pas:610</summary>
    public TCheckBoxSeam chkAttackTeleportAfterDamage = null!;
    /// <summary>原文 uFrmCustomMagic.pas:611</summary>
    public TCheckBoxSeam chkAttackTeleportRush = null!;
    /// <summary>原文 uFrmCustomMagic.pas:612</summary>
    public TSpinEditExSeam seAttackTeleportRushCount = null!;
    /// <summary>原文 uFrmCustomMagic.pas:613</summary>
    public TCheckBoxSeam chkAttackTeleportAttack = null!;
    /// <summary>原文 uFrmCustomMagic.pas:614</summary>
    public TLabelSeam lblAttackWidth = null!;
    /// <summary>原文 uFrmCustomMagic.pas:615</summary>
    public TLabelSeam lblH_AttackWidth = null!;
    /// <summary>原文 uFrmCustomMagic.pas:616</summary>
    public TSpinEditExSeam seAttackLineWidth = null!;
}
