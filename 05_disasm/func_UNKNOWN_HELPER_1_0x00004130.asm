; Function: UNKNOWN_HELPER_1 at 0x00004130
; Instructions: 25
; Direct calls: 0

0x00004130: push     ebp
0x00004131: mov      ebp, esp
0x00004133: mov      al, byte ptr [ebp + 8]
0x00004136: push     esi
0x00004137: test     al, al
0x00004139: mov      esi, ecx
0x0000413b: je       0x4161
0x0000413d: mov      eax, dword ptr [esi + 4]
0x00004140: test     eax, eax
0x00004142: je       0x4161
0x00004144: lea      ecx, [eax - 1]
0x00004147: mov      al, byte ptr [eax - 1]
0x0000414a: test     al, al
0x0000414c: je       0x4158
0x0000414e: cmp      al, 0xff
0x00004150: je       0x4158
0x00004152: dec      al
0x00004154: mov      byte ptr [ecx], al
0x00004156: jmp      0x4161
0x00004161: mov      dword ptr [esi + 4], 0
0x00004168: mov      dword ptr [esi + 8], 0
0x0000416f: mov      dword ptr [esi + 0xc], 0
0x00004176: pop      esi
0x00004177: pop      ebp
0x00004178: ret      4
