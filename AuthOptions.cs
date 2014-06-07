using System;

[Flags]
public enum AuthOptions
{
    SearchDown = 1,
    SearchInclusive = 8,
    SearchReverse = 0x10,
    SearchUp = 2
}

