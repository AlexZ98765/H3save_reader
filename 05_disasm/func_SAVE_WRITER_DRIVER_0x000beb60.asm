; Function: SAVE_WRITER_DRIVER at 0x000beb60
; Image base: 0x00400000
; Instructions: 113
; Direct calls: 7
; Indirect calls: 2

0x000beb60: push     ebp
0x000beb61: mov      ebp, esp
0x000beb63: push     -1
0x000beb65: push     0x62c5b0
0x000beb6a: mov      eax, dword ptr fs:[0]
0x000beb70: push     eax
0x000beb71: mov      dword ptr fs:[0], esp
0x000beb78: sub      esp, 0x448
0x000beb7e: push     ebx
0x000beb7f: push     esi
0x000beb80: mov      dword ptr [ebp - 0x14], ecx
0x000beb83: push     edi
0x000beb84: mov      ecx, 0x57
0x000beb89: xor      eax, eax
0x000beb8b: lea      edi, [ebp - 0x2f3]
0x000beb91: mov      byte ptr [ebp - 0x2f4], 0
0x000beb98: rep stosd dword ptr es:[edi], eax
0x000beb9a: stosw    word ptr es:[edi], ax
0x000beb9c: mov      ecx, 0x57
0x000beba1: xor      eax, eax
0x000beba3: lea      edi, [ebp - 0x193]
0x000beba9: mov      byte ptr [ebp - 0x194], 0
0x000bebb0: rep stosd dword ptr es:[edi], eax
0x000bebb2: mov      dword ptr [ebp - 0x10], esp
0x000bebb5: stosw    word ptr es:[edi], ax
0x000bebb7: call     dword ptr [0x63a354]  ; winmm.dll!timeGetTime
0x000bebbd: mov      dword ptr [ebp - 0x24], eax
0x000bebc0: mov      al, byte ptr [ebp + 0x10]
0x000bebc3: test     al, al
0x000bebc5: jne      0xbebd6
0x000bebc7: mov      ecx, dword ptr [0x6992b8]
0x000bebcd: push     1
0x000bebcf: push     0
0x000bebd1: call     0x175e0  ; → sub_000175e0
0x000bebd6: mov      al, byte ptr [ebp + 0xc]
0x000bebd9: test     al, al
0x000bebdb: je       0xbec80
0x000bebe1: mov      ebx, dword ptr [ebp + 8]
0x000bebe4: or       ecx, 0xffffffff
0x000bebe7: mov      edi, ebx
0x000bebe9: xor      eax, eax
0x000bebeb: repne scasb al, byte ptr es:[edi]
0x000bebed: not      ecx
0x000bebef: sub      edi, ecx
0x000bebf1: lea      edx, [ebp - 0x2f4]
0x000bebf7: mov      eax, ecx
0x000bebf9: mov      esi, edi
0x000bebfb: mov      edi, edx
0x000bebfd: push     0x6603ec
0x000bec02: shr      ecx, 2
0x000bec05: rep movsd dword ptr es:[edi], dword ptr [esi]
0x000bec07: mov      ecx, eax
0x000bec09: and      ecx, 3
0x000bec0c: rep movsb byte ptr es:[edi], byte ptr [esi]
0x000bec0e: lea      ecx, [ebp - 0x2f4]
0x000bec14: push     ecx
0x000bec15: call     0x217fbb  ; → sub_00217fbb
0x000bec1a: mov      al, byte ptr [0x69779c]
0x000bec1f: add      esp, 8
0x000bec22: test     al, al
0x000bec24: je       0xbec2d
0x000bec26: push     0x677da4
0x000bec2b: jmp      0xbec3f
0x000bec3f: lea      edx, [ebp - 0x2f4]
0x000bec45: lea      eax, [ebp - 0x194]
0x000bec4b: push     edx
0x000bec4c: push     0x677d98  ; "%s.%s"
0x000bec51: push     eax
0x000bec52: call     0x2179de  ; → sub_002179de
0x000bec57: add      esp, 0x10
0x000bec5a: jmp      0xbeca8
0x000beca8: mov      al, byte ptr [ebp + 0x18]
0x000becab: test     al, al
0x000becad: je       0xbecd4
0x000becaf: lea      ecx, [ebp - 0x194]
0x000becb5: lea      edx, [ebp - 0x454]
0x000becbb: push     ecx
0x000becbc: push     0x677d88  ; ".\DATA\"
0x000becc1: push     0x660358
0x000becc6: push     edx
0x000becc7: call     0x2179de  ; → sub_002179de
0x000beccc: add      esp, 0x10
0x000beccf: jmp      0xbed65
0x000bed65: mov      cl, byte ptr [ebp + 0x14]
0x000bed68: mov      eax, 0x677d80
0x000bed6d: test     cl, cl
0x000bed6f: jne      0xbed76
0x000bed71: mov      eax, 0x677d78
0x000bed76: lea      ecx, [ebp - 0x454]
0x000bed7c: push     eax
0x000bed7d: push     ecx
0x000bed7e: lea      ecx, [ebp - 0x1c]
0x000bed81: mov      dword ptr [ebp - 4], 0
0x000bed88: call     0xd6eb0  ; → sub_000d6eb0
0x000bed8d: mov      ecx, dword ptr [ebp - 0x14]
0x000bed90: lea      edx, [ebp - 0x1c]
0x000bed93: push     edx
0x000bed94: mov      byte ptr [ebp - 4], 1
0x000bed98: call     0xbe0b0  ; → sub_000be0b0
0x000bed9d: lea      ecx, [ebp - 0x1c]
0x000beda0: mov      byte ptr [ebp - 4], 0
0x000beda4: call     0xd6fc0  ; → sub_000d6fc0
0x000beda9: call     dword ptr [0x63a354]  ; winmm.dll!timeGetTime
0x000bedaf: mov      dword ptr [ebp - 0x20], eax
0x000bedb2: mov      al, 1
0x000bedb4: mov      ecx, dword ptr [ebp - 0xc]
0x000bedb7: mov      dword ptr fs:[0], ecx
0x000bedbe: pop      edi
0x000bedbf: pop      esi
0x000bedc0: pop      ebx
0x000bedc1: mov      esp, ebp
0x000bedc3: pop      ebp
0x000bedc4: ret      0x14
