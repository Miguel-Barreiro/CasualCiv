using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Calcatz.AnyObjectFinder {

    public static class StringUtility {

        public static bool CompareString(string stringValue, string searchTerm, bool caseSensitive, bool orderMatters = false) {
            string[] searchSplits;
            if (caseSensitive) searchSplits = null;
            else searchSplits = ObjectNames.NicifyVariableName(searchTerm).ToLower().Split(' ');
            return CompareString(stringValue, searchTerm, caseSensitive, searchSplits, orderMatters);
        }

        public static bool CompareString(string stringValue, string searchTerm, bool caseSensitive, string[] searchSplits, bool orderMatters = false) {
            if (string.IsNullOrEmpty(searchTerm)) return true;
            if (stringValue == null) return false;
            if (caseSensitive) {
                return stringValue.Contains(searchTerm);
            }
            else {
                bool found = stringValue.ToLower().Contains(searchTerm.ToLower());
                if (!found) {
                    found = true;
                    string nicifyLowerName = ObjectNames.NicifyVariableName(stringValue).ToLower();
                    if (orderMatters) {
                        found = false;
                        var nicifiedSplits = nicifyLowerName.Split(' ', '.');
                        if (searchSplits.Length > 0) {
                            for (int i = 0; i < nicifiedSplits.Length; i++) {
                                if (nicifiedSplits[i] == searchSplits[0]) {
                                    bool subFound = true;
                                    for (int j = 0; j < searchSplits.Length; j++) {
                                        if (i + j >= nicifiedSplits.Length) {
                                            if (j == 0) subFound = false;
                                            break;
                                        }
                                        if (nicifiedSplits[i + j] != searchSplits[j]) {
                                            subFound = false;
                                        }
                                    }
                                    if (subFound) {
                                        found = true;
                                        break;
                                    }
                                }
                                else {
                                    continue;
                                }
                            }
                        }
                    }
                    else {
                        for (int i = 0; i < searchSplits.Length; i++) {
                            if (searchSplits[i] == "") continue;
                            if (!nicifyLowerName.Contains(searchSplits[i])) {
                                found = false;
                                break;
                            }
                        }
                    }
                }
                if (found) {
                    return true;
                }
                return false;
            }
        }

    }

}