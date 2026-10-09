; Function: dword ptr [edx + 0x24] at 0x0017f600
; Image base: 0x00400000
; Instructions: 178

0x0017f600: push     ebp
0x0017f601: mov      ebp, esp
0x0017f603: push     -1
0x0017f605: push     0x633050
0x0017f60a: mov      eax, dword ptr fs:[0]
0x0017f610: push     eax
0x0017f611: mov      dword ptr fs:[0], esp
0x0017f618: sub      esp, 0x8c
0x0017f61e: mov      eax, dword ptr [0x699538]
0x0017f623: push     ebx
0x0017f624: mov      ebx, ecx
0x0017f626: push     esi
0x0017f627: movsx    ecx, byte ptr [eax + 0x1f843]
0x0017f62e: mov      dword ptr [ebx + 0x378], ecx
0x0017f634: mov      eax, dword ptr [0x69959c]
0x0017f639: xor      esi, esi
0x0017f63b: push     edi
0x0017f63c: cmp      eax, esi
0x0017f63e: mov      dword ptr [ebp - 0x14], ebx
0x0017f641: je       0x17f851
0x0017f647: push     0x69
0x0017f649: mov      ecx, ebx
0x0017f64b: call     0x1ff5b0
0x0017f650: push     6
0x0017f652: push     6
0x0017f654: mov      ecx, eax
0x0017f656: call     0x1fed80
0x0017f65b: cmp      dword ptr [0x69959c], esi
0x0017f661: je       0x17f967
0x0017f667: mov      ecx, dword ptr [0x69d858]
0x0017f66d: mov      edx, dword ptr [ecx]
0x0017f66f: call     dword ptr [edx + 0x90]
0x0017f675: test     al, al
0x0017f677: jne      0x17f967
0x0017f67d: mov      ecx, dword ptr [0x699538]
0x0017f683: call     0xbee60
0x0017f688: mov      al, byte ptr [ebx + 0x1865]
0x0017f68e: mov      byte ptr [ebx + 0x1864], 1
0x0017f695: test     al, al
0x0017f697: je       0x17f6b0
0x0017f699: push     0xb3
0x0017f69e: mov      ecx, ebx
0x0017f6a0: call     0x1ff5b0
0x0017f6a5: push     6
0x0017f6a7: push     6
0x0017f6a9: mov      ecx, eax
0x0017f6ab: call     0x1fed80
0x0017f6b0: lea      ecx, [ebp - 0x84]
0x0017f6b6: mov      dword ptr [ebp - 0x90], 0x410
0x0017f6c0: mov      dword ptr [ebp - 0x98], 0xffffffff
0x0017f6ca: mov      dword ptr [ebp - 0x8c], 0x48
0x0017f6d4: mov      dword ptr [ebp - 0x94], esi
0x0017f6da: mov      dword ptr [ebp - 0x88], esi
0x0017f6e0: call     0x17f9f0
0x0017f6e5: mov      ecx, 8
0x0017f6ea: mov      esi, 0x69d658
0x0017f6ef: lea      edi, [ebp - 0x84]
0x0017f6f5: lea      eax, [ebx + 0x1874]
0x0017f6fb: rep movsd dword ptr es:[edi], dword ptr [esi]
0x0017f6fd: push     0x14
0x0017f6ff: lea      ecx, [ebp - 0x64]
0x0017f702: push     eax
0x0017f703: push     ecx
0x0017f704: call     0x218fe0
0x0017f709: add      esp, 0xc
0x0017f70c: xor      esi, esi
0x0017f70e: xor      edx, edx
0x0017f710: lea      ecx, [ebp - 0x98]
0x0017f716: push     1
0x0017f718: push     esi
0x0017f719: call     0x154950
0x0017f71e: mov      dword ptr [ebp - 0x28], 0x640f34
0x0017f725: mov      dword ptr [ebp - 0x24], 0x19
0x0017f72c: mov      dword ptr [ebp - 0x18], esi
0x0017f72f: mov      dword ptr [ebp - 0x1c], esi
0x0017f732: mov      dword ptr [ebp - 0x20], esi
0x0017f735: mov      ecx, dword ptr [0x69d858]
0x0017f73b: push     esi
0x0017f73c: lea      eax, [ebp - 0x28]
0x0017f73f: push     esi
0x0017f740: mov      edx, dword ptr [ecx]
0x0017f742: push     eax
0x0017f743: mov      dword ptr [ebp - 4], esi
0x0017f746: call     dword ptr [edx + 0x70]
0x0017f749: mov      eax, dword ptr [ebp - 0x18]
0x0017f74c: mov      dword ptr [ebp - 0x10], esi
0x0017f74f: cmp      esi, eax
0x0017f751: jae      0x17f809
0x0017f757: jb       0x17f75d
0x0017f759: xor      edi, edi
0x0017f75b: jmp      0x17f763
0x0017f763: mov      ecx, dword ptr [0x69d858]
0x0017f769: mov      eax, dword ptr [edi + 0x100]
0x0017f76f: push     0
0x0017f771: push     0
0x0017f773: mov      edx, dword ptr [ecx]
0x0017f775: push     eax
0x0017f776: call     dword ptr [edx + 0x48]
0x0017f779: mov      ebx, eax
0x0017f77b: test     ebx, ebx
0x0017f77d: setne    al
0x0017f780: mov      byte ptr [ebp - 0x30], al
0x0017f783: mov      dword ptr [ebp - 0x2c], ebx
0x0017f786: test     ebx, ebx
0x0017f788: mov      byte ptr [ebp - 4], 1
0x0017f78c: jne      0x17f7a0
0x0017f78e: test     al, al
0x0017f790: mov      byte ptr [ebp - 4], bl
0x0017f793: je       0x17f7f7
0x0017f795: push     ebx
0x0017f796: call     0x20b0f0
0x0017f79b: add      esp, 4
0x0017f79e: jmp      0x17f7f7
0x0017f7f7: mov      eax, dword ptr [ebp - 0x18]
0x0017f7fa: inc      esi
0x0017f7fb: cmp      esi, eax
0x0017f7fd: mov      dword ptr [ebp - 0x10], esi
0x0017f800: jb       0x17f75d
0x0017f806: mov      ebx, dword ptr [ebp - 0x14]
0x0017f809: xor      esi, esi
0x0017f80b: mov      dword ptr [ebp - 4], 0xffffffff
0x0017f812: test     eax, eax
0x0017f814: mov      dword ptr [ebp - 0x28], 0x640f34
0x0017f81b: jbe      0x17f838
0x0017f81d: mov      edx, dword ptr [ebp - 0x28]
0x0017f820: push     esi
0x0017f821: lea      ecx, [ebp - 0x28]
0x0017f824: call     dword ptr [edx + 8]
0x0017f827: push     eax
0x0017f828: call     0x20b0f0
0x0017f82d: mov      eax, dword ptr [ebp - 0x18]
0x0017f830: add      esp, 4
0x0017f833: inc      esi
0x0017f834: cmp      esi, eax
0x0017f836: jb       0x17f81d
0x0017f838: mov      eax, dword ptr [ebp - 0x20]
0x0017f83b: test     eax, eax
0x0017f83d: je       0x17f973
0x0017f843: push     eax
0x0017f844: call     0x20b0f0
0x0017f849: add      esp, 4
0x0017f84c: jmp      0x17f973
0x0017f973: mov      eax, dword ptr [0x69959c]
0x0017f978: test     eax, eax
0x0017f97a: je       0x17f98e
0x0017f97c: mov      ecx, dword ptr [0x69d858]
0x0017f982: mov      edx, dword ptr [ecx]
0x0017f984: call     dword ptr [edx + 0x90]
0x0017f98a: test     al, al
0x0017f98c: je       0x17f9bf
0x0017f98e: lea      eax, [ebx + 0x1030]
0x0017f994: mov      ecx, ebx
0x0017f996: push     eax
0x0017f997: call     0x182e10
0x0017f99c: push     0x68338c  ; 'NEWGAME.gm1'
0x0017f9a1: mov      ecx, ebx
0x0017f9a3: mov      byte ptr [ebx + 0x1864], 1
0x0017f9aa: call     0x18d6b0
0x0017f9af: mov      ecx, dword ptr [ebx + 0x374]
0x0017f9b5: push     0
0x0017f9b7: push     ecx
0x0017f9b8: mov      ecx, ebx
0x0017f9ba: call     0x1857d0
0x0017f9bf: mov      ebx, dword ptr [ebx + 0x1840]
0x0017f9c5: test     ebx, ebx
0x0017f9c7: je       0x17f9d2
0x0017f9c9: mov      edx, dword ptr [ebx]
0x0017f9cb: push     0
0x0017f9cd: mov      ecx, ebx
0x0017f9cf: call     dword ptr [edx + 0x24]
0x0017f9d2: mov      ecx, dword ptr [ebp - 0xc]
0x0017f9d5: pop      edi
0x0017f9d6: pop      esi
0x0017f9d7: pop      ebx
0x0017f9d8: mov      dword ptr fs:[0], ecx
0x0017f9df: mov      esp, ebp
0x0017f9e1: pop      ebp
0x0017f9e2: ret      
