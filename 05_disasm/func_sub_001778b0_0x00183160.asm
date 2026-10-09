; Function: sub_001778b0 at 0x00183160
; Image base: 0x00400000
; Instructions: 121

0x00183160: push     ebp
0x00183161: mov      ebp, esp
0x00183163: push     ecx
0x00183164: push     ebx
0x00183165: mov      ebx, ecx
0x00183167: push     esi
0x00183168: push     edi
0x00183169: mov      edi, dword ptr [ebp + 8]
0x0018316c: lea      esi, [ebx + 0x1050]
0x00183172: cmp      esi, edi
0x00183174: mov      dword ptr [ebp - 4], ebx
0x00183177: je       0x1832e4
0x0018317d: mov      eax, dword ptr [edi + 4]
0x00183180: test     eax, eax
0x00183182: jne      0x183189
0x00183184: mov      dword ptr [ebp + 8], eax
0x00183187: jmp      0x1831a2
0x001831a2: mov      ecx, dword ptr [esi + 4]
0x001831a5: test     ecx, ecx
0x001831a7: jne      0x1831ad
0x001831a9: xor      edx, edx
0x001831ab: jmp      0x1831c3
0x001831c3: cmp      dword ptr [ebp + 8], edx
0x001831c6: ja       0x18321c
0x001831c8: mov      edx, dword ptr [edi + 8]
0x001831cb: push     ecx
0x001831cc: mov      ecx, dword ptr [edi + 4]
0x001831cf: call     0x18f450
0x001831d4: mov      ecx, dword ptr [esi + 8]
0x001831d7: push     ecx
0x001831d8: push     eax
0x001831d9: mov      ecx, esi
0x001831db: call     0x18f370
0x001831e0: mov      eax, dword ptr [edi + 4]
0x001831e3: test     eax, eax
0x001831e5: jne      0x1831eb
0x001831e7: xor      edx, edx
0x001831e9: jmp      0x183201
0x00183201: lea      eax, [edx + edx*4]
0x00183204: lea      eax, [eax + eax*8]
0x00183207: lea      ecx, [eax + eax*8]
0x0018320a: shl      ecx, 1
0x0018320c: sub      ecx, edx
0x0018320e: mov      edx, dword ptr [esi + 4]
0x00183211: lea      eax, [edx + ecx*4]
0x00183214: mov      dword ptr [esi + 8], eax
0x00183217: jmp      0x1832e4
0x001832e4: mov      dword ptr [ebx + 0x374], 0
0x001832ee: mov      eax, dword ptr [esi + 4]
0x001832f1: test     eax, eax
0x001832f3: je       0x18330d
0x001832f5: mov      ecx, dword ptr [esi + 8]
0x001832f8: sub      ecx, eax
0x001832fa: mov      eax, 0x5102371
0x001832ff: imul     ecx
0x00183301: sar      edx, 6
0x00183304: mov      eax, edx
0x00183306: shr      eax, 0x1f
0x00183309: add      edx, eax
0x0018330b: jne      0x18331b
0x0018330d: mov      al, byte ptr [ebx + 0x64]
0x00183310: test     al, al
0x00183312: je       0x18331b
0x00183314: mov      byte ptr [0x69fdfc], 1
0x0018331b: push     0
0x0018331d: push     0
0x0018331f: push     2
0x00183321: mov      ecx, ebx
0x00183323: mov      byte ptr [ebx + 0x36c], 0
0x0018332a: mov      dword ptr [ebx + 0x185c], 1
0x00183334: call     0x185320
0x00183339: mov      eax, dword ptr [esi + 4]
0x0018333c: test     eax, eax
0x0018333e: je       0x18342b
0x00183344: mov      ecx, dword ptr [esi + 8]
0x00183347: sub      ecx, eax
0x00183349: mov      eax, 0x5102371
0x0018334e: imul     ecx
0x00183350: sar      edx, 6
0x00183353: mov      ecx, edx
0x00183355: shr      ecx, 0x1f
0x00183358: add      edx, ecx
0x0018335a: je       0x18342b
0x00183360: mov      eax, dword ptr [esi + 4]
0x00183363: test     eax, eax
0x00183365: jne      0x18336b
0x00183367: xor      edx, edx
0x00183369: jmp      0x183381
0x00183381: mov      esi, dword ptr [0x69fe24]
0x00183387: mov      ecx, dword ptr [ebx + 0x183c]
0x0018338d: sub      edx, esi
0x0018338f: mov      eax, dword ptr [ecx]
0x00183391: inc      edx
0x00183392: push     edx
0x00183393: call     dword ptr [eax + 0x34]
0x00183396: mov      al, byte ptr [ebx + 0x65]
0x00183399: test     al, al
0x0018339b: jne      0x1833ab
0x0018339d: mov      al, byte ptr [ebx + 0x64]
0x001833a0: test     al, al
0x001833a2: jne      0x1833ab
0x001833a4: push     0x683288  ; 'Arrogance.h3m'
0x001833a9: jmp      0x1833b0
0x001833b0: mov      ecx, ebx
0x001833b2: call     0x18d6b0
0x001833b7: mov      cl, byte ptr [ebx + 0x65]
0x001833ba: test     cl, cl
0x001833bc: je       0x1833ed
0x001833be: test     al, al
0x001833c0: jne      0x1833ed
0x001833c2: mov      dword ptr [ebx + 0x374], 0xffffffff
0x001833cc: mov      edx, dword ptr [0x69fe20]
0x001833d2: mov      ecx, dword ptr [0x699538]
0x001833d8: call     0x1778b0
0x001833dd: pop      edi
0x001833de: mov      byte ptr [ebx + 0x1834], 1
0x001833e5: pop      esi
0x001833e6: pop      ebx
0x001833e7: mov      esp, ebp
0x001833e9: pop      ebp
0x001833ea: ret      4
