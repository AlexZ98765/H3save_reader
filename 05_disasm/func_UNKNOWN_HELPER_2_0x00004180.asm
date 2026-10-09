; Function: UNKNOWN_HELPER_2 at 0x00004180
; Instructions: 30
; Direct calls: 1

0x00004180: push     ebp
0x00004181: mov      ebp, esp
0x00004183: push     ebx
0x00004184: push     esi
0x00004185: mov      esi, dword ptr [ebp + 0xc]
0x00004188: mov      ebx, ecx
0x0000418a: cmp      esi, -3
0x0000418d: jbe      0x4194
0x0000418f: call     0x20b0fb  ; → sub_0020b0fb
0x00004194: mov      ecx, dword ptr [ebx + 4]
0x00004197: xor      edx, edx
0x00004199: cmp      ecx, edx
0x0000419b: je       0x41c2
0x0000419d: mov      al, byte ptr [ecx - 1]
0x000041a0: test     al, al
0x000041a2: je       0x41c2
0x000041a4: cmp      al, 0xff
0x000041a6: je       0x41c2
0x000041a8: cmp      esi, edx
0x000041aa: jne      0x41ec
0x000041ac: dec      al
0x000041ae: pop      esi
0x000041af: mov      byte ptr [ecx - 1], al
0x000041b2: mov      dword ptr [ebx + 4], edx
0x000041b5: mov      dword ptr [ebx + 8], edx
0x000041b8: mov      dword ptr [ebx + 0xc], edx
0x000041bb: mov      eax, ebx
0x000041bd: pop      ebx
0x000041be: pop      ebp
0x000041bf: ret      8
