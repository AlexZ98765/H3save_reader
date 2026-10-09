; Function: SAVE_WRITER_CONTENT at 0x000bc290
; Instructions: 43
; Direct calls: 0

0x000bc290: push     ebp
0x000bc291: mov      ebp, esp
0x000bc293: sub      esp, 0x184
0x000bc299: push     ebx
0x000bc29a: mov      ebx, dword ptr [ebp + 8]
0x000bc29d: push     esi
0x000bc29e: mov      esi, ecx
0x000bc2a0: mov      eax, dword ptr [ebx]
0x000bc2a2: push     8
0x000bc2a4: push     esi
0x000bc2a5: mov      ecx, ebx
0x000bc2a7: mov      dword ptr [ebp - 4], esi
0x000bc2aa: call     dword ptr [eax + 8]
0x000bc2ad: mov      ecx, dword ptr [esi + 8]
0x000bc2b0: mov      edx, dword ptr [ebx]
0x000bc2b2: lea      eax, [ebp + 8]
0x000bc2b5: mov      dword ptr [ebp + 8], ecx
0x000bc2b8: push     4
0x000bc2ba: push     eax
0x000bc2bb: mov      ecx, ebx
0x000bc2bd: call     dword ptr [edx + 8]
0x000bc2c0: mov      ecx, dword ptr [esi + 0xc]
0x000bc2c3: mov      edx, dword ptr [ebx]
0x000bc2c5: lea      eax, [ebp + 8]
0x000bc2c8: mov      dword ptr [ebp + 8], ecx
0x000bc2cb: push     4
0x000bc2cd: push     eax
0x000bc2ce: mov      ecx, ebx
0x000bc2d0: call     dword ptr [edx + 8]
0x000bc2d3: mov      edx, dword ptr [ebx]
0x000bc2d5: lea      eax, [ebp - 0x24]
0x000bc2d8: push     0x20
0x000bc2da: push     eax
0x000bc2db: mov      ecx, ebx
0x000bc2dd: call     dword ptr [edx + 8]
0x000bc2e0: cmp      eax, 0x20
0x000bc2e3: jae      0xbc2f0
0x000bc2e5: pop      esi
0x000bc2e6: or       eax, 0xffffffff
0x000bc2e9: pop      ebx
0x000bc2ea: mov      esp, ebp
0x000bc2ec: pop      ebp
0x000bc2ed: ret      4
