; Function: SAVE_READER_DRIVER at 0x000beff0
; Image base: 0x00400000
; Instructions: 58
; Direct calls: 4
; Indirect calls: 0

0x000beff0: push     ebp
0x000beff1: mov      ebp, esp
0x000beff3: push     -1
0x000beff5: push     0x62c5c8
0x000beffa: mov      eax, dword ptr fs:[0]
0x000bf000: push     eax
0x000bf001: mov      dword ptr fs:[0], esp
0x000bf008: sub      esp, 0x1d4
0x000bf00e: push     ebx
0x000bf00f: push     esi
0x000bf010: push     edi
0x000bf011: mov      edi, ecx
0x000bf013: mov      dword ptr [ebp - 0x10], esp
0x000bf016: mov      dword ptr [ebp - 0x14], edi
0x000bf019: call     0xbee60  ; → sub_000bee60
0x000bf01e: mov      eax, dword ptr [ebp + 0xc]
0x000bf021: xor      ebx, ebx
0x000bf023: cmp      eax, ebx
0x000bf025: jne      0xbf216
0x000bf02b: mov      esi, dword ptr [ebp + 8]
0x000bf02e: push     3
0x000bf030: push     0x677da8
0x000bf035: push     esi
0x000bf036: mov      dword ptr [0x697308], ebx
0x000bf03c: call     0x226680  ; → sub_00226680
0x000bf041: add      esp, 0xc
0x000bf044: test     eax, eax
0x000bf046: push     esi
0x000bf047: jne      0xbf05c
0x000bf049: push     0x677d88  ; ".\DATA\"
0x000bf04e: lea      eax, [ebp - 0x1e0]
0x000bf054: push     0x660358
0x000bf059: push     eax
0x000bf05a: jmp      0xbf06d
0x000bf06d: call     0x2179de  ; → sub_002179de
0x000bf072: add      esp, 0x10
0x000bf075: lea      edx, [ebp - 0x1e0]
0x000bf07b: lea      ecx, [ebp - 0x1c]
0x000bf07e: mov      dword ptr [ebp - 4], ebx
0x000bf081: push     0x677d6c
0x000bf086: push     edx
0x000bf087: call     0xd6eb0  ; → sub_000d6eb0
0x000bf08c: mov      byte ptr [ebp - 4], 1
0x000bf090: cmp      ebx, 8
0x000bf093: jge      0xbf110
0x000bf095: mov      eax, ebx
0x000bf097: shl      eax, 4
0x000bf09a: mov      ecx, dword ptr [eax + edi + 0x4e684]
0x000bf0a1: lea      esi, [eax + edi + 0x4e67c]
0x000bf0a8: mov      eax, ecx
0x000bf0aa: mov      edi, dword ptr [esi + 4]
0x000bf0ad: cmp      eax, ecx
0x000bf0af: je       0xbf0bd
0x000bf0b1: mov      edx, dword ptr [eax]
0x000bf0b3: mov      dword ptr [edi], edx
0x000bf0b5: add      edi, 4
0x000bf0b8: add      eax, 4
0x000bf0bb: jmp      0xbf0ad
