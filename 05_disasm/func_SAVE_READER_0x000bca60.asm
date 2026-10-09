; Function: SAVE_READER at 0x000bca60
; Image base: 0x00400000
; Instructions: 71
; Direct calls: 8
; Indirect calls: 0

0x000bca60: push     ebp
0x000bca61: mov      ebp, esp
0x000bca63: push     -1
0x000bca65: push     0x62c1ea
0x000bca6a: mov      eax, dword ptr fs:[0]
0x000bca70: push     eax
0x000bca71: mov      dword ptr fs:[0], esp
0x000bca78: sub      esp, 0x7a8
0x000bca7e: push     ebx
0x000bca7f: mov      dword ptr [ebp - 0x14], ecx
0x000bca82: push     esi
0x000bca83: push     edi
0x000bca84: lea      ecx, [ebp - 0x5c4]
0x000bca8a: call     0x5a480  ; → sub_0005a480
0x000bca8f: lea      ecx, [ebp - 0x2c0]
0x000bca95: mov      dword ptr [ebp - 4], 0
0x000bca9c: call     0x5a900  ; → sub_0005a900
0x000bcaa1: lea      ecx, [ebp - 0xf0]
0x000bcaa7: call     0x89040  ; → sub_00089040
0x000bcaac: mov      al, byte ptr [ebp + 0xb]
0x000bcaaf: push     0
0x000bcab1: lea      ecx, [ebp - 0x74]
0x000bcab4: mov      byte ptr [ebp - 4], 1
0x000bcab8: mov      byte ptr [ebp - 0x74], al
0x000bcabb: call     0x4130  ; → sub_00004130
0x000bcac0: xor      ecx, ecx
0x000bcac2: mov      edi, 0x677d38  ; "H3SVG"
0x000bcac7: mov      dword ptr [ebp - 0x5d4], ecx
0x000bcacd: xor      eax, eax
0x000bcacf: mov      dword ptr [ebp - 0x5d0], ecx
0x000bcad5: or       ecx, 0xffffffff
0x000bcad8: repne scasb al, byte ptr es:[edi]
0x000bcada: not      ecx
0x000bcadc: sub      edi, ecx
0x000bcade: lea      edx, [ebp - 0x5d4]
0x000bcae4: mov      eax, ecx
0x000bcae6: mov      esi, edi
0x000bcae8: mov      edi, edx
0x000bcaea: shr      ecx, 2
0x000bcaed: rep movsd dword ptr es:[edi], dword ptr [esi]
0x000bcaef: mov      ecx, eax
0x000bcaf1: and      ecx, 3
0x000bcaf4: rep movsb byte ptr es:[edi], byte ptr [esi]
0x000bcaf6: mov      dword ptr [ebp - 0x5cc], 0x2a
0x000bcb00: mov      ebx, dword ptr [ebp + 8]
0x000bcb03: lea      ecx, [ebp - 0x5d4]
0x000bcb09: push     ebx
0x000bcb0a: mov      dword ptr [ebp - 4], 2
0x000bcb11: call     0xbc410  ; → sub_000bc410
0x000bcb16: test     eax, eax
0x000bcb18: je       0xbcb34
0x000bcb1a: push     1
0x000bcb1c: lea      ecx, [ebp - 0x74]
0x000bcb1f: mov      dword ptr [ebp - 4], 4
0x000bcb26: call     0x4130  ; → sub_00004130
0x000bcb2b: mov      byte ptr [ebp - 4], 3
0x000bcb2f: jmp      0xbd848
0x000bd848: lea      ecx, [ebp - 0xf0]
0x000bd84e: call     0x5ecf0  ; → sub_0005ecf0
0x000bd853: lea      ecx, [ebp - 0x5c4]
0x000bd859: mov      dword ptr [ebp - 4], 0xffffffff
0x000bd860: call     0x5a9e0  ; → sub_0005a9e0
0x000bd865: pop      edi
0x000bd866: pop      esi
0x000bd867: or       eax, 0xffffffff
0x000bd86a: pop      ebx
0x000bd86b: mov      ecx, dword ptr [ebp - 0xc]
0x000bd86e: mov      dword ptr fs:[0], ecx
0x000bd875: mov      esp, ebp
0x000bd877: pop      ebp
0x000bd878: ret      4
