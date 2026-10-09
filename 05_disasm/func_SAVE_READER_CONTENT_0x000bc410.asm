; Function: SAVE_READER_CONTENT at 0x000bc410
; Instructions: 89
; Direct calls: 7

0x000bc410: push     ebp
0x000bc411: mov      ebp, esp
0x000bc413: push     -1
0x000bc415: push     0x62be86
0x000bc41a: mov      eax, dword ptr fs:[0]
0x000bc420: push     eax
0x000bc421: mov      dword ptr fs:[0], esp
0x000bc428: sub      esp, 0x1a8
0x000bc42e: mov      al, byte ptr [ebp + 0xb]
0x000bc431: push     ebx
0x000bc432: push     esi
0x000bc433: push     edi
0x000bc434: mov      esi, ecx
0x000bc436: xor      edi, edi
0x000bc438: mov      dword ptr [ebp - 0x10], esp
0x000bc43b: push     edi
0x000bc43c: lea      ecx, [ebp - 0x2c]
0x000bc43f: mov      dword ptr [ebp - 0x30], esi
0x000bc442: mov      byte ptr [ebp - 0x2c], al
0x000bc445: call     0x4130  ; → sub_00004130
0x000bc44a: mov      ebx, dword ptr [ebp + 8]
0x000bc44d: mov      dword ptr [ebp - 4], edi
0x000bc450: cmp      ebx, edi
0x000bc452: mov      byte ptr [ebp - 0x1c], 0
0x000bc456: setne    al
0x000bc459: mov      byte ptr [ebp - 0x11], al
0x000bc45c: mov      dword ptr [ebp - 0x18], edi
0x000bc45f: test     al, al
0x000bc461: mov      byte ptr [ebp - 4], 1
0x000bc465: jne      0xbc513
0x000bc46b: mov      ecx, dword ptr [0x699538]
0x000bc471: xor      eax, eax
0x000bc473: lea      edx, [ecx + 0x1f6d9]
0x000bc479: or       ecx, 0xffffffff
0x000bc47c: mov      edi, edx
0x000bc47e: repne scasb al, byte ptr es:[edi]
0x000bc480: not      ecx
0x000bc482: dec      ecx
0x000bc483: push     ecx
0x000bc484: push     edx
0x000bc485: lea      ecx, [ebp - 0x2c]
0x000bc488: call     0x4180  ; → sub_00004180
0x000bc48d: push     0x677d70  ; "games"
0x000bc492: call     0x219aa0  ; → sub_00219aa0
0x000bc497: push     8
0x000bc499: mov      byte ptr [ebp - 4], 2
0x000bc49d: call     0x217492  ; → sub_00217492
0x000bc4a2: add      esp, 8
0x000bc4a5: mov      dword ptr [ebp + 8], eax
0x000bc4a8: test     eax, eax
0x000bc4aa: mov      byte ptr [ebp - 4], 3
0x000bc4ae: je       0xbc4cd
0x000bc4b0: mov      edx, dword ptr [ebp - 0x28]
0x000bc4b3: test     edx, edx
0x000bc4b5: jne      0xbc4bc
0x000bc4b7: mov      edx, 0x63a608
0x000bc4bc: push     0x677d6c
0x000bc4c1: push     edx
0x000bc4c2: mov      ecx, eax
0x000bc4c4: call     0xd6eb0  ; → sub_000d6eb0
0x000bc4c9: mov      ebx, eax
0x000bc4cb: jmp      0xbc4cf
0x000bc4cf: test     ebx, ebx
0x000bc4d1: setne    al
0x000bc4d4: test     ebx, ebx
0x000bc4d6: je       0xbc4dd
0x000bc4d8: mov      byte ptr [ebp - 0x1c], al
0x000bc4db: jmp      0xbc4e5
0x000bc4e5: push     0x6755a0
0x000bc4ea: mov      dword ptr [ebp - 0x18], ebx
0x000bc4ed: mov      dword ptr [ebp - 4], 1
0x000bc4f4: call     0x219aa0  ; → sub_00219aa0
0x000bc4f9: add      esp, 4
0x000bc4fc: test     ebx, ebx
0x000bc4fe: jne      0xbc510
0x000bc500: jmp      0xbc62c
0x000bc62c: push     1
0x000bc62e: lea      ecx, [ebp - 0x2c]
0x000bc631: mov      dword ptr [ebp - 4], 0xffffffff
0x000bc638: call     0x4130  ; → sub_00004130
0x000bc63d: or       eax, 0xffffffff
0x000bc640: mov      ecx, dword ptr [ebp - 0xc]
0x000bc643: mov      dword ptr fs:[0], ecx
0x000bc64a: pop      edi
0x000bc64b: pop      esi
0x000bc64c: pop      ebx
0x000bc64d: mov      esp, ebp
0x000bc64f: pop      ebp
0x000bc650: ret      4
