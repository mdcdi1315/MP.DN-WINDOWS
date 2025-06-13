
#include "VersionHelper.h"

static inline BOOL __IsDigit(WCHAR ch)
{
	return ch >= '0' && ch <= '9';
}

static BOOL __ParsePositiveIntFromStringKnownOriginAndLength(LPWSTR str, int origin , int length , int* presult)
{
	if (presult == NULL) { return FALSE; }
	length = length > 10 ? 10 : length;
	int finalnum = 0 , shift = 1;
	for (int I = (length + origin) - 1; I >= origin; I--)
	{
		if (__IsDigit(str[I]) == FALSE) { return FALSE; }
		finalnum += (str[I] - 48) * shift;
		shift *= 10;
	}
	*presult = finalnum;
	return TRUE;
}

static inline BOOL VersionHasFlag(DECOMPOSED_VERSION_FLAGS flagscurrent , DECOMPOSED_VERSION_FLAGS expected)
{
	return (flagscurrent & expected) == expected;
}

static inline BOOL VersionFlagsMatchTo(DECOMPOSED_VERSION_FLAGS flags1, DECOMPOSED_VERSION_FLAGS flags2, DECOMPOSED_VERSION_FLAGS expected)
{
	return VersionHasFlag(flags1, expected) && VersionHasFlag(flags2, expected);
}

PDECOMPOSED_VERSION GetDecomposedVersionFromString(LPWSTR string)
{
	if (string == NULL) { return NULL; }
	int len = StringLength(string);
	int l2 = 0, origin = 0;
	int vernum = 0;
	PDECOMPOSED_VERSION dcc = SafeAllocate(sizeof(DECOMPOSED_VERSION));
	SafeZeroMemory(dcc, sizeof(DECOMPOSED_VERSION));
	for (int I = 0; I < len; I++)
	{
		if (vernum > 3) { break; } // Should consider it as parse error but it is enough for our needs.
		if (string[I] == '.') {
			if (origin == I) { 
				// .. format found , parse error
				SafeUnallocate(dcc);
				return FALSE; 
			}
			int ret;
			BOOL result = __ParsePositiveIntFromStringKnownOriginAndLength(string, origin, l2, &ret);
			if (result == FALSE) { 
				// string is not consisting of digits, stop
				SafeUnallocate(dcc);
				return FALSE; 
			}
			// switch through cases and define flags appropriately
			switch (vernum)
			{
				case 0:
					dcc->Major = ret;
					dcc->Flags |= HasMajor;
					break;
				case 1:
					dcc->Minor = ret;
					dcc->Flags |= HasMinor;
					break;
				case 2:
					dcc->Build = ret;
					dcc->Flags |= HasBuild;
					break;
				case 3:
					dcc->Revision = ret;
					dcc->Flags |= HasRevision;
					break;
			}
			// set new origin and zero length and rerun
			origin = I + 1;
			l2 = 0;
			// update version structure number to fill in
			vernum++;
		} else {
			l2++;
		}
	}
	return dcc; // Return the decomposed structure
}

LPWSTR GetStringFromDecomposedVersion(PDECOMPOSED_VERSION decversion)
{
	if (decversion == NULL) { return NULL; }
	if (VersionHasFlag(decversion->Flags, HasMajor))
	{
		return FormatString(L"%d", decversion->Major);
	} else if (VersionHasFlag(decversion->Flags, HasMajor | HasMinor))
	{
		return FormatString(L"%d.%d" , decversion->Major , decversion->Minor);
	} else if (VersionHasFlag(decversion->Flags, HasMajor | HasMinor | HasRevision))
	{
		return FormatString(L"%d.%d.%d", decversion->Major, decversion->Minor, decversion->Revision);
	} else if (VersionHasFlag(decversion->Flags, HasMajor | HasMinor | HasRevision | HasBuild))
	{
		return FormatString(L"%d.%d.%d.%d", decversion->Major, decversion->Minor, decversion->Revision , decversion->Build);
	} else
	{
		return NULL;
	}
}

PDECOMPOSED_VERSION GetDecomposedVersion(int major, int minor, int build, int revision)
{
	PDECOMPOSED_VERSION dcc = SafeAllocate(sizeof(DECOMPOSED_VERSION));
	SafeZeroMemory(dcc, sizeof(DECOMPOSED_VERSION));
	if (major >= 0)
	{
		dcc->Major = major;
		dcc->Flags |= HasMajor;
		if (minor >= 0)
		{
			dcc->Minor = minor;
			dcc->Flags |= HasMinor;
			if (build >= 0)
			{
				dcc->Build = build;
				dcc->Flags |= HasBuild;
				if (revision >= 0)
				{
					dcc->Revision = revision;
					dcc->Flags |= HasRevision;
				}
			}
		}
	}
	return dcc;
}

BOOL DecVersionIsEqualTo(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two)
{
	if (one == NULL || two == NULL) { return FALSE; }
	if (one->Flags != two->Flags) { return FALSE; }
	if (VersionHasFlag(one->Flags, HasMajor | HasMinor | HasBuild | HasRevision))
	{
		return one->Major == two->Major && one->Minor == two->Minor && one->Revision == two->Revision && one->Build == two->Build;
	} else if (VersionHasFlag(one->Flags, HasMajor | HasMinor | HasBuild))
	{
		return one->Major == two->Major && one->Minor == two->Minor && one->Build == two->Build;
	}
	else if (VersionHasFlag(one->Flags, HasMajor | HasMinor))
	{
		return one->Major == two->Major && one->Minor == two->Minor;
	}
	else if (VersionHasFlag(one->Flags, HasMajor))
	{
		return one->Major == two->Major;
	}
	else
	{
		return FALSE;
	}
}

BOOL DecVersionIsGreaterThan(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two)
{
	if (one == NULL || two == NULL) { return FALSE; }
	if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor | HasMinor | HasBuild | HasRevision))
	{
		return one->Major > two->Major && one->Minor > two->Minor && one->Revision > two->Revision && one->Build > two->Build;
	}
	else if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor | HasMinor | HasBuild))
	{
		return one->Major > two->Major && one->Minor > two->Minor && one->Build > two->Build;
	}
	else if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor | HasMinor))
	{
		return one->Major > two->Major && one->Minor > two->Minor;
	}
	else if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor))
	{
		return one->Major > two->Major;
	}
	else {
		return FALSE;
	}
}

BOOL DecVersionIsLessTo(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two)
{
	if (one == NULL || two == NULL) { return FALSE; }
	if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor | HasMinor | HasBuild | HasRevision))
	{
		return one->Major < two->Major && one->Minor < two->Minor && one->Revision < two->Revision && one->Build < two->Build;
	}
	else if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor | HasMinor | HasBuild))
	{
		return one->Major < two->Major && one->Minor < two->Minor && one->Build < two->Build;
	}
	else if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor | HasMinor))
	{
		return one->Major < two->Major && one->Minor < two->Minor;
	}
	else if (VersionFlagsMatchTo(one->Flags, two->Flags, HasMajor))
	{
		return one->Major < two->Major;
	}
	else {
		return FALSE;
	}
}

