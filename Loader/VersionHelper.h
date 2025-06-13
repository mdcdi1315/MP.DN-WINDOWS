#pragma once

#include "Helpers.h"

typedef enum decomposed_version_flags {
	HasMajor = 1,
	HasMinor = 2,
	HasBuild = 4,
	HasRevision = 8
} DECOMPOSED_VERSION_FLAGS , *PDECOMPOSED_VERSION_FLAGS;

typedef struct decomposed_version {
	int Major;
	int Minor;
	int Build;
	int Revision;
	DECOMPOSED_VERSION_FLAGS Flags;
} DECOMPOSED_VERSION , *PDECOMPOSED_VERSION;

// From a string that represents a typed version, this function returns a DECOMPOSED_VERSION structure 
// of the given string that represents the parsed version. Returns NULL on any failure.
PDECOMPOSED_VERSION GetDecomposedVersionFromString(LPWSTR string);

// From a pointer to a DECOMPOSED_VERSION structure , this function returns back the string parsed with GetDecomposedVersionFromString.
LPWSTR GetStringFromDecomposedVersion(PDECOMPOSED_VERSION decversion);

// Creates a new DECOMPOSED_VERSION structure from the specified build numbers.
// Fill any one of these parameters with any negative value to effectively discard them.
// Regardlessly of the filled in version numbers, SafeUnallocate must be ALWAYS called to free the returned pointer.
PDECOMPOSED_VERSION GetDecomposedVersion(int major, int minor, int build, int revision);

// Tests whether two DECOMPOSED_VERSION structure pointers represent the same version.
// Will always return false if their flags are different, or if one of or both pointers are NULL.
BOOL DecVersionIsEqualTo(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two);

// Tests whether the first DECOMPOSED_VERSION structure pointer is,
// as in versioning terms, 'later' than the second pointer.
BOOL DecVersionIsGreaterThan(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two);

// Tests whether the first DECOMPOSED_VERSION structure pointer is,
// as in versioning terms, 'before' to the second pointer.
BOOL DecVersionIsLessTo(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two);

// Tests whether the first DECOMPOSED_VERSION structure pointer is,
// as in versioning terms, 'before' to, or equal to the second pointer.
inline BOOL DecVersionIsLessOrEqualTo(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two)
{
	// Declare it inline so that this body is replaced in the actual function calls.
	return DecVersionIsLessTo(one, two) || DecVersionIsEqualTo(one, two);
}

// Tests whether the first DECOMPOSED_VERSION structure pointer is,
// as in versioning terms, 'later' than, or equal to the second pointer.
inline BOOL DecVersionIsGreaterThanOrEqualTo(PDECOMPOSED_VERSION one, PDECOMPOSED_VERSION two)
{
	// Declare it inline so that this body is replaced in the actual function calls.
	return DecVersionIsGreaterThan(one, two) || DecVersionIsEqualTo(one, two);
}
