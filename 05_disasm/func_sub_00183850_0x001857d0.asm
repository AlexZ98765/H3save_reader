; Function: sub_00183850 at 0x001857d0
; Image base: 0x00400000
; Instructions: 56

0x001857d0: push     ebp
0x001857d1: mov      ebp, esp
0x001857d3: sub      esp, 0x818
0x001857d9: push     ebx
0x001857da: mov      ebx, ecx
0x001857dc: push     esi
0x001857dd: push     edi
0x001857de: mov      eax, dword ptr [ebx + 0x1054]
0x001857e4: xor      edi, edi
0x001857e6: cmp      eax, edi
0x001857e8: jne      0x1857ee
0x001857ea: xor      edx, edx
0x001857ec: jmp      0x185807
0x00185807: mov      eax, dword ptr [ebp + 8]
0x0018580a: cmp      eax, edx
0x0018580c: jge      0x185f4b
0x00185812: mov      ecx, dword ptr [ebx + 0x374]
0x00185818: mov      dword ptr [ebp - 0x20], edi
0x0018581b: cmp      ecx, eax
0x0018581d: mov      dword ptr [ebp - 0x1c], edi
0x00185820: mov      dword ptr [ebp - 0x18], edi
0x00185823: mov      dword ptr [ebp - 0x14], edi
0x00185826: mov      dword ptr [ebp - 0x10], edi
0x00185829: mov      dword ptr [ebp - 0xc], edi
0x0018582c: mov      dword ptr [ebp - 8], edi
0x0018582f: mov      dword ptr [ebp - 4], edi
0x00185832: jne      0x18583f
0x00185834: mov      cl, byte ptr [ebx + 0x65]
0x00185837: test     cl, cl
0x00185839: jne      0x18583f
0x0018583b: xor      ecx, ecx
0x0018583d: jmp      0x185844
0x00185844: mov      byte ptr [ebx + 0x1854], cl
0x0018584a: mov      cl, byte ptr [ebx + 0x37f]
0x00185850: cmp      eax, -1
0x00185853: mov      dword ptr [ebx + 0x374], eax
0x00185859: jne      0x185921
0x0018585f: test     cl, cl
0x00185861: jne      0x185925
0x00185867: mov      al, byte ptr [ebx + 0x65]
0x0018586a: test     al, al
0x0018586c: je       0x185909
0x00185872: mov      ecx, ebx
0x00185874: call     0x183850
0x00185879: mov      ecx, dword ptr [0x699538]
0x0018587f: mov      esi, 4
0x00185884: mov      dword ptr [ebp - 0x20], 0x200
0x0018588b: mov      dword ptr [ebp - 0x18], 0xbd
0x00185892: mov      dword ptr [ebp - 0x1c], esi
0x00185895: mov      eax, dword ptr [ecx + 0x1f884]
0x0018589b: add      eax, -0x24
0x0018589e: cmp      eax, 0x6c
0x001858a1: ja       0x1858d2
0x001858a3: xor      edx, edx
0x001858a5: mov      dl, byte ptr [eax + 0x585f68]
0x001858ab: jmp      dword ptr [edx*4 + 0x585f54]
