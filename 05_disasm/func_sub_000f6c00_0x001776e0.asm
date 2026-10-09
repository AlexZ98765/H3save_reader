; Function: sub_000f6c00 at 0x001776e0
; Image base: 0x00400000
; Instructions: 31

0x001776e0: push     ebp
0x001776e1: mov      ebp, esp
0x001776e3: sub      esp, 0xc8
0x001776e9: push     ebx
0x001776ea: push     esi
0x001776eb: mov      esi, ecx
0x001776ed: xor      bl, bl
0x001776ef: call     0x10c990
0x001776f4: cmp      eax, 0x100000
0x001776f9: jae      0x17772f
0x001776fb: mov      eax, dword ptr [0x6a5dc4]
0x00177700: push     0
0x00177702: push     -1
0x00177704: push     0
0x00177706: mov      ecx, dword ptr [eax + 0x20]
0x00177709: push     -1
0x0017770b: push     0
0x0017770d: push     -1
0x0017770f: mov      ecx, dword ptr [ecx + 0xb14]
0x00177715: push     0
0x00177717: push     -1
0x00177719: push     -1
0x0017771b: push     -1
0x0017771d: mov      edx, 1
0x00177722: call     0xf6c00
0x00177727: xor      al, al
0x00177729: pop      esi
0x0017772a: pop      ebx
0x0017772b: mov      esp, ebp
0x0017772d: pop      ebp
0x0017772e: ret      
