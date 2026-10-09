; Function: SAVE_WRITER at 0x000be0b0
; Image base: 0x00400000
; Instructions: 73
; Direct calls: 9
; Indirect calls: 0

0x000be0b0: push     ebp
0x000be0b1: mov      ebp, esp
0x000be0b3: push     -1
0x000be0b5: push     0x62c593
0x000be0ba: mov      eax, dword ptr fs:[0]
0x000be0c0: push     eax
0x000be0c1: mov      dword ptr fs:[0], esp
0x000be0c8: sub      esp, 0x5c0
0x000be0ce: push     ebx
0x000be0cf: mov      ebx, ecx
0x000be0d1: push     esi
0x000be0d2: push     edi
0x000be0d3: lea      ecx, [ebp - 0x5bc]
0x000be0d9: call     0x5a480  ; → sub_0005a480
0x000be0de: lea      ecx, [ebp - 0x2b8]
0x000be0e4: mov      dword ptr [ebp - 4], 0
0x000be0eb: call     0x5a900  ; → sub_0005a900
0x000be0f0: lea      ecx, [ebp - 0xe8]
0x000be0f6: call     0x89040  ; → sub_00089040
0x000be0fb: mov      al, byte ptr [ebp + 0xb]
0x000be0fe: push     0
0x000be100: lea      ecx, [ebp - 0x6c]
0x000be103: mov      byte ptr [ebp - 4], 1
0x000be107: mov      byte ptr [ebp - 0x6c], al
0x000be10a: call     0x4130  ; → sub_00004130
0x000be10f: xor      ecx, ecx
0x000be111: mov      edi, 0x677d38  ; "H3SVG"
0x000be116: mov      dword ptr [ebp - 0x5cc], ecx
0x000be11c: xor      eax, eax
0x000be11e: mov      dword ptr [ebp - 0x5c8], ecx
0x000be124: or       ecx, 0xffffffff
0x000be127: repne scasb al, byte ptr es:[edi]
0x000be129: not      ecx
0x000be12b: sub      edi, ecx
0x000be12d: lea      edx, [ebp - 0x5cc]
0x000be133: mov      eax, ecx
0x000be135: mov      esi, edi
0x000be137: mov      edi, edx
0x000be139: shr      ecx, 2
0x000be13c: rep movsd dword ptr es:[edi], dword ptr [esi]
0x000be13e: mov      ecx, eax
0x000be140: and      ecx, 3
0x000be143: rep movsb byte ptr es:[edi], byte ptr [esi]
0x000be145: mov      dword ptr [ebp - 0x5c4], 0x2a
0x000be14f: lea      ecx, [ebp - 0x5cc]
0x000be155: mov      dword ptr [ebp - 4], 2
0x000be15c: call     0xbc010  ; → sub_000bc010
0x000be161: mov      esi, dword ptr [ebp + 8]
0x000be164: lea      ecx, [ebp - 0x5cc]
0x000be16a: push     esi
0x000be16b: call     0xbc290  ; → sub_000bc290
0x000be170: test     eax, eax
0x000be172: jge      0xbe18e
0x000be174: push     1
0x000be176: lea      ecx, [ebp - 0x6c]
0x000be179: mov      dword ptr [ebp - 4], 4
0x000be180: call     0x4130  ; → sub_00004130
0x000be185: mov      byte ptr [ebp - 4], 3
0x000be189: jmp      0xbea6b
0x000bea6b: lea      ecx, [ebp - 0xe8]
0x000bea71: call     0x5ecf0  ; → sub_0005ecf0
0x000bea76: lea      ecx, [ebp - 0x5bc]
0x000bea7c: mov      dword ptr [ebp - 4], 0xffffffff
0x000bea83: call     0x5a9e0  ; → sub_0005a9e0
0x000bea88: pop      edi
0x000bea89: pop      esi
0x000bea8a: or       eax, 0xffffffff
0x000bea8d: pop      ebx
0x000bea8e: mov      ecx, dword ptr [ebp - 0xc]
0x000bea91: mov      dword ptr fs:[0], ecx
0x000bea98: mov      esp, ebp
0x000bea9a: pop      ebp
0x000bea9b: ret      4
