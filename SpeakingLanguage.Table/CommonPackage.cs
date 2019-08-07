using System.Runtime.InteropServices;

namespace SpeakingLanguage.Table
{
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CommonPackage
{
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EventPackage
{
public int Index;
public String16 MainBgColor;
public bool UseBonusPackage;
public String16 BonusPackageEventRewardCode;
public String16 BonusBgColor;
public String16 MaplePackage;
}

public String16 Code;
public String64 CombinedPackageCode;
public String64 TitleLanguage;
public String64 SubTitleLanguage1;
public String64 SubTitleLanguage2;
public String64 SubTitleLanguage3;
public String16 SubTitleLanguage4;
public String64 RemainTimeFormatLanguage;
public String16 TimerType;
public bool IsVisibleMainItem;
public bool IsVisibleInTown;
public String64 BackgroundTexture;
public String32 ImgSpriteName;
public String64 SubTexture;
public String16 Color;
public String64 BonusAwardInfoLanguage;
public String64 DecoTexture;
public String16 Layout;
public String16 Size;
public String16 BackgroundColor;
public EventPackage EventPackage_00;
public EventPackage EventPackage_01;
public EventPackage EventPackage_02;
public EventPackage EventPackage_03;
public EventPackage EventPackage_04;
}
}
