; Function: sub_000bbda0 at 0x00179ce0
; Image base: 0x00400000
; Instructions: 3421

0x00179ce0: push     ebp
0x00179ce1: mov      ebp, esp
0x00179ce3: push     -1
0x00179ce5: push     0x632d8e
0x00179cea: mov      eax, dword ptr fs:[0]
0x00179cf0: push     eax
0x00179cf1: mov      dword ptr fs:[0], esp
0x00179cf8: sub      esp, 0x314
0x00179cfe: push     ebx
0x00179cff: push     esi
0x00179d00: push     edi
0x00179d01: push     0
0x00179d03: push     0x258
0x00179d08: push     0x320
0x00179d0d: mov      ebx, ecx
0x00179d0f: push     0
0x00179d11: push     0
0x00179d13: mov      dword ptr [ebp - 0x20], ebx
0x00179d16: call     0x1afa0
0x00179d1b: lea      esi, [ebx + 0x38c]
0x00179d21: mov      dword ptr [ebp - 4], 0
0x00179d28: mov      ecx, esi
0x00179d2a: mov      dword ptr [ebp - 0x10], esi
0x00179d2d: call     0x5a480
0x00179d32: lea      ecx, [esi + 0x304]
0x00179d38: mov      byte ptr [ebp - 4], 1
0x00179d3c: call     0x5a900
0x00179d41: lea      ecx, [esi + 0x700]
0x00179d47: call     0xbbda0
0x00179d4c: mov      ecx, 0xf
0x00179d51: xor      eax, eax
0x00179d53: lea      edi, [esi + 0x58c]
0x00179d59: rep stosd dword ptr es:[edi], eax
0x00179d5b: stosb    byte ptr es:[edi], al
0x00179d5c: mov      ecx, 0x4b
0x00179d61: xor      eax, eax
0x00179d63: lea      edi, [esi + 0x5c9]
0x00179d69: rep stosd dword ptr es:[edi], eax
0x00179d6b: stosb    byte ptr es:[edi], al
0x00179d6c: mov      byte ptr [esi + 0x33c], 1
0x00179d73: mov      al, byte ptr [ebp + 0xb]
0x00179d76: xor      edi, edi
0x00179d78: mov      byte ptr [ebx + 0x1030], al
0x00179d7e: mov      dword ptr [ebx + 0x1034], edi
0x00179d84: mov      dword ptr [ebx + 0x1038], edi
0x00179d8a: mov      dword ptr [ebx + 0x103c], edi
0x00179d90: mov      cl, byte ptr [ebp + 0xb]
0x00179d93: mov      byte ptr [ebx + 0x1040], cl
0x00179d99: mov      dword ptr [ebx + 0x1044], edi
0x00179d9f: mov      dword ptr [ebx + 0x1048], edi
0x00179da5: mov      dword ptr [ebx + 0x104c], edi
0x00179dab: mov      dl, byte ptr [ebp + 0xb]
0x00179dae: mov      byte ptr [ebx + 0x1050], dl
0x00179db4: mov      dword ptr [ebx + 0x1054], edi
0x00179dba: mov      dword ptr [ebx + 0x1058], edi
0x00179dc0: mov      dword ptr [ebx + 0x105c], edi
0x00179dc6: lea      esi, [ebx + 0x1064]
0x00179dcc: mov      byte ptr [ebp - 4], 5
0x00179dd0: mov      dword ptr [ebp - 0x14], esi
0x00179dd3: mov      dword ptr [ebp - 0x18], 8
0x00179dda: mov      ecx, dword ptr [ebp - 0x14]
0x00179ddd: call     0x17cb10
0x00179de2: mov      ecx, dword ptr [ebp - 0x14]
0x00179de5: mov      eax, dword ptr [ebp - 0x18]
0x00179de8: add      ecx, 0x7c
0x00179deb: dec      eax
0x00179dec: mov      dword ptr [ebp - 0x14], ecx
0x00179def: mov      dword ptr [ebp - 0x18], eax
0x00179df2: jne      0x179dda
0x00179df4: lea      eax, [esi + 0x3e0]
0x00179dfa: mov      dword ptr [ebp - 0x18], 8
0x00179e01: mov      dword ptr [ebp - 0x14], eax
0x00179e04: mov      ecx, dword ptr [ebp - 0x14]
0x00179e07: call     0x17cb10
0x00179e0c: mov      ecx, dword ptr [ebp - 0x14]
0x00179e0f: mov      eax, dword ptr [ebp - 0x18]
0x00179e12: add      ecx, 0x7c
0x00179e15: dec      eax
0x00179e16: mov      dword ptr [ebp - 0x14], ecx
0x00179e19: mov      dword ptr [ebp - 0x18], eax
0x00179e1c: jne      0x179e04
0x00179e1e: or       eax, 0xffffffff
0x00179e21: mov      dword ptr [esi + 0x7c4], edi
0x00179e27: mov      dword ptr [esi + 0x7c0], eax
0x00179e2d: mov      dword ptr [esi + 0x7c8], eax
0x00179e33: mov      dword ptr [esi + 0x7cc], eax
0x00179e39: xor      edx, edx
0x00179e3b: add      esi, 0x3e4
0x00179e41: mov      dword ptr [ebp - 0x14], esi
0x00179e44: mov      ecx, dword ptr [ebp - 0x14]
0x00179e47: mov      dword ptr [ecx - 0x370], edx
0x00179e4d: mov      eax, dword ptr [0x6a5dc4]
0x00179e52: or       ecx, 0xffffffff
0x00179e55: mov      eax, dword ptr [eax + 0x20]
0x00179e58: mov      edi, dword ptr [eax + 0x754]
0x00179e5e: xor      eax, eax
0x00179e60: repne scasb al, byte ptr es:[edi]
0x00179e62: not      ecx
0x00179e64: sub      edi, ecx
0x00179e66: mov      eax, ecx
0x00179e68: mov      esi, edi
0x00179e6a: mov      edi, dword ptr [ebp - 0x14]
0x00179e6d: shr      ecx, 2
0x00179e70: rep movsd dword ptr es:[edi], dword ptr [esi]
0x00179e72: mov      ecx, eax
0x00179e74: and      ecx, 3
0x00179e77: inc      edx
0x00179e78: rep movsb byte ptr es:[edi], byte ptr [esi]
0x00179e7a: mov      ecx, dword ptr [ebp - 0x14]
0x00179e7d: add      ecx, 0x7c
0x00179e80: cmp      edx, 8
0x00179e83: mov      dword ptr [ebp - 0x14], ecx
0x00179e86: jl       0x179e44
0x00179e88: lea      esi, [ebx + 0x1888]
0x00179e8e: mov      ecx, esi
0x00179e90: call     0x157ba0
0x00179e95: mov      dword ptr [esi], 0x641cf8
0x00179e9b: mov      byte ptr [esi + 0xc], 0
0x00179e9f: mov      byte ptr [ebp - 4], 6
0x00179ea3: mov      dword ptr [ebx], 0x641cbc
0x00179ea9: mov      eax, dword ptr [0x69fe28]
0x00179eae: xor      esi, esi
0x00179eb0: cmp      eax, esi
0x00179eb2: jne      0x179f13
0x00179eb4: mov      ecx, dword ptr [0x6992b0]
0x00179eba: push     1
0x00179ebc: push     1
0x00179ebe: call     0x10cea0
0x00179ec3: push     esi
0x00179ec4: push     esi
0x00179ec5: push     1
0x00179ec7: push     esi
0x00179ec8: call     dword ptr [0x63a0d0]
0x00179ece: cmp      eax, esi
0x00179ed0: mov      dword ptr [0x69fe2c], eax
0x00179ed5: je       0x179f13
0x00179ed7: mov      ecx, dword ptr [0x699414]
0x00179edd: call     0x19aad0
0x00179ee2: lea      ecx, [ebp - 0x10]
0x00179ee5: push     ecx
0x00179ee6: push     esi
0x00179ee7: push     esi
0x00179ee8: push     0x577ac0
0x00179eed: push     esi
0x00179eee: push     esi
0x00179eef: call     0x21a433
0x00179ef4: add      esp, 0x18
0x00179ef7: cmp      eax, esi
0x00179ef9: mov      dword ptr [0x69fe28], eax
0x00179efe: jne      0x179f13
0x00179f00: mov      edx, dword ptr [0x69fe2c]
0x00179f06: push     edx
0x00179f07: call     dword ptr [0x63a0c8]
0x00179f0d: mov      dword ptr [0x69fe2c], esi
0x00179f13: mov      eax, dword ptr [0x6992d0]
0x00179f18: cmp      dword ptr [eax + 0x38], 0x65
0x00179f1c: je       0x179f2e
0x00179f1e: mov      al, byte ptr [0x69779c]
0x00179f23: mov      byte ptr [0x69fdec], 0
0x00179f2a: test     al, al
0x00179f2c: je       0x179f35
0x00179f2e: mov      byte ptr [0x69fdec], 1
0x00179f35: mov      dword ptr [0x69fc44], ebx
0x00179f3b: mov      byte ptr [ebx + 0x65], 0
0x00179f3f: mov      byte ptr [ebx + 0x64], 0
0x00179f43: mov      byte ptr [ebx + 0x66], 0
0x00179f47: mov      ecx, dword ptr [0x69928c]
0x00179f4d: mov      dword ptr [0x69fe24], 0x12
0x00179f57: mov      edx, dword ptr [ecx]
0x00179f59: mov      dword ptr [ebx + 0x1898], edx
0x00179f5f: mov      dword ptr [ebx + 0x189c], 0x155
0x00179f69: mov      eax, dword ptr [0x69928c]
0x00179f6e: mov      eax, dword ptr [eax]
0x00179f70: cmp      eax, 1
0x00179f73: je       0x179f7a
0x00179f75: cmp      eax, 3
0x00179f78: jne      0x179f84
0x00179f7a: mov      dword ptr [ebx + 0x189c], 0x156
0x00179f84: mov      eax, dword ptr [ebp + 8]
0x00179f87: cmp      eax, 1
0x00179f8a: jne      0x179f91
0x00179f8c: mov      byte ptr [ebx + 0x64], al
0x00179f8f: jmp      0x179ff5
0x00179ff5: mov      al, byte ptr [ebx + 0x64]
0x00179ff8: test     al, al
0x00179ffa: jne      0x17a021
0x00179ffc: mov      al, byte ptr [ebx + 0x65]
0x00179fff: test     al, al
0x0017a001: jne      0x17a021
0x0017a003: cmp      dword ptr [0x69959c], esi
0x0017a009: je       0x17a02f
0x0017a00b: mov      ecx, dword ptr [0x69d858]
0x0017a011: cmp      ecx, esi
0x0017a013: je       0x17a027
0x0017a015: lea      edx, [ebx + 0x1888]
0x0017a01b: push     edx
0x0017a01c: call     0x153b70
0x0017a021: mov      ecx, dword ptr [0x69d858]
0x0017a027: cmp      dword ptr [0x69959c], esi
0x0017a02d: jne      0x17a035
0x0017a02f: mov      byte ptr [ebp + 8], 1
0x0017a033: jmp      0x17a040
0x0017a040: mov      ecx, dword ptr [ebp + 8]
0x0017a043: and      ecx, 0xff
0x0017a049: push     ecx
0x0017a04a: push     0x6836d8  ; 'TSingleSelectionWindow::IsHost()=%d'
0x0017a04f: push     0x69d698
0x0017a054: call     0x11bd60
0x0017a059: mov      byte ptr [0x69fdfc], 0
0x0017a060: mov      byte ptr [ebx + 0x1834], 0
0x0017a067: mov      dword ptr [ebx + 0x1848], esi
0x0017a06d: mov      dword ptr [ebx + 0x184c], esi
0x0017a073: mov      dword ptr [ebx + 0x1850], esi
0x0017a079: mov      byte ptr [ebx + 0x1854], 1
0x0017a080: mov      dword ptr [ebx + 0x1858], esi
0x0017a086: mov      byte ptr [ebx + 0x36c], 0
0x0017a08d: mov      dword ptr [ebx + 0x185c], esi
0x0017a093: mov      dword ptr [ebx + 0x1860], esi
0x0017a099: mov      dword ptr [ebx + 0x388], esi
0x0017a09f: mov      byte ptr [ebx + 0x1865], 0
0x0017a0a6: mov      byte ptr [ebx + 0x186c], 0
0x0017a0ad: mov      dword ptr [ebx + 0x380], esi
0x0017a0b3: mov      dword ptr [ebx + 0x1840], esi
0x0017a0b9: mov      edx, dword ptr [0x699538]
0x0017a0bf: push     0x44
0x0017a0c1: movsx    eax, byte ptr [edx + 0x1f843]
0x0017a0c8: mov      dword ptr [ebx + 0x378], eax
0x0017a0ce: call     0x217492
0x0017a0d3: add      esp, 0x10
0x0017a0d6: mov      dword ptr [ebp + 8], eax
0x0017a0d9: cmp      eax, esi
0x0017a0db: mov      byte ptr [ebp - 4], 8
0x0017a0df: je       0x17a0f1
0x0017a0e1: push     0x19
0x0017a0e3: push     0x136
0x0017a0e8: mov      ecx, eax
0x0017a0ea: call     0x1576d0
0x0017a0ef: jmp      0x17a0f3
0x0017a0f3: lea      ecx, [ebp - 0x320]
0x0017a0f9: push     0x15f
0x0017a0fe: push     ecx
0x0017a0ff: mov      byte ptr [ebp - 4], 6
0x0017a103: push     esi
0x0017a104: mov      dword ptr [ebx + 0x1870], eax
0x0017a10a: call     dword ptr [0x63a100]
0x0017a110: lea      edx, [ebp - 0x320]
0x0017a116: lea      ecx, [ebp + 8]
0x0017a119: push     edx
0x0017a11a: call     0x1ef1a0
0x0017a11f: mov      al, byte ptr [ebp + 0xb]
0x0017a122: push     esi
0x0017a123: lea      ecx, [ebp - 0x38]
0x0017a126: mov      byte ptr [ebp - 4], 9
0x0017a12a: mov      byte ptr [ebp - 0x38], al
0x0017a12d: call     0x4130
0x0017a132: lea      ecx, [ebp - 0x38]
0x0017a135: mov      byte ptr [ebp - 4], 0xa
0x0017a139: push     ecx
0x0017a13a: push     0x6834ec  ; 'ProductVersion'
0x0017a13f: lea      ecx, [ebp + 8]
0x0017a142: call     0x1ef200
0x0017a147: test     al, al
0x0017a149: je       0x17a17c
0x0017a14b: mov      edi, dword ptr [ebp - 0x34]
0x0017a14e: cmp      edi, esi
0x0017a150: jne      0x17a157
0x0017a152: mov      edi, 0x63a608
0x0017a157: or       ecx, 0xffffffff
0x0017a15a: xor      eax, eax
0x0017a15c: repne scasb al, byte ptr es:[edi]
0x0017a15e: not      ecx
0x0017a160: sub      edi, ecx
0x0017a162: mov      edx, ecx
0x0017a164: mov      esi, edi
0x0017a166: lea      edi, [ebx + 0x1874]
0x0017a16c: shr      ecx, 2
0x0017a16f: rep movsd dword ptr es:[edi], dword ptr [esi]
0x0017a171: mov      ecx, edx
0x0017a173: and      ecx, 3
0x0017a176: rep movsb byte ptr es:[edi], byte ptr [esi]
0x0017a178: xor      esi, esi
0x0017a17a: jmp      0x17a183
0x0017a183: push     1
0x0017a185: lea      ecx, [ebp - 0x38]
0x0017a188: mov      byte ptr [ebp - 4], 9
0x0017a18c: call     0x4130
0x0017a191: lea      ecx, [ebp + 8]
0x0017a194: mov      byte ptr [ebp - 4], 6
0x0017a198: call     0x1ef1f0
0x0017a19d: mov      byte ptr [ebx + 0x37c], 0
0x0017a1a4: mov      byte ptr [ebx + 0x1864], 0
0x0017a1ab: mov      byte ptr [ebx + 0x37d], 0
0x0017a1b2: mov      byte ptr [ebx + 0x37e], 0
0x0017a1b9: mov      byte ptr [ebx + 0x37f], 0
0x0017a1c0: call     0xf8970
0x0017a1c5: mov      dword ptr [ebx + 0x60], eax
0x0017a1c8: mov      al, byte ptr [ebx + 0x65]
0x0017a1cb: test     al, al
0x0017a1cd: jne      0x17a1ee
0x0017a1cf: mov      eax, dword ptr [0x699538]
0x0017a1d4: lea      ecx, [esi + eax + 0x20ad0]
0x0017a1db: call     0xb9ae0
0x0017a1e0: add      esi, 0x168
0x0017a1e6: cmp      esi, 0xb40
0x0017a1ec: jl       0x17a1cf
0x0017a1ee: mov      eax, dword ptr [ebx + 0x34]
0x0017a1f1: lea      esi, [ebx + 0x30]
0x0017a1f4: test     eax, eax
0x0017a1f6: je       0x17a205
0x0017a1f8: mov      ecx, dword ptr [esi + 0xc]
0x0017a1fb: sub      ecx, eax
0x0017a1fd: sar      ecx, 2
0x0017a200: cmp      ecx, 0x6f
0x0017a203: jae      0x17a283
0x0017a205: push     0x1bc
0x0017a20a: call     0x217492
0x0017a20f: mov      dword ptr [ebp - 0x18], eax
0x0017a212: mov      dword ptr [ebp + 8], eax
0x0017a215: mov      eax, dword ptr [esi + 8]
0x0017a218: add      esp, 4
0x0017a21b: mov      dword ptr [ebp - 0x14], eax
0x0017a21e: mov      edi, dword ptr [esi + 4]
0x0017a221: cmp      edi, eax
0x0017a223: je       0x17a242
0x0017a225: mov      ecx, dword ptr [ebp + 8]
0x0017a228: mov      edx, edi
0x0017a22a: call     0xaf880
0x0017a22f: mov      edx, dword ptr [ebp + 8]
0x0017a232: mov      eax, dword ptr [ebp - 0x14]
0x0017a235: add      edi, 4
0x0017a238: add      edx, 4
0x0017a23b: cmp      edi, eax
0x0017a23d: mov      dword ptr [ebp + 8], edx
0x0017a240: jne      0x17a225
0x0017a242: lea      eax, [esi + 8]
0x0017a245: lea      edi, [esi + 4]
0x0017a248: mov      dword ptr [ebp + 8], eax
0x0017a24b: mov      ecx, esi
0x0017a24d: mov      edx, dword ptr [eax]
0x0017a24f: mov      eax, dword ptr [edi]
0x0017a251: push     edx
0x0017a252: push     eax
0x0017a253: call     0xaf240
0x0017a258: mov      eax, dword ptr [edi]
0x0017a25a: push     eax
0x0017a25b: call     0x20b0f0
0x0017a260: mov      ecx, dword ptr [ebp - 0x18]
0x0017a263: add      esp, 4
0x0017a266: add      ecx, 0x1bc
0x0017a26c: mov      dword ptr [esi + 0xc], ecx
0x0017a26f: mov      ecx, esi
0x0017a271: call     0x14d2b0
0x0017a276: mov      ecx, dword ptr [ebp - 0x18]
0x0017a279: lea      edx, [ecx + eax*4]
0x0017a27c: mov      eax, dword ptr [ebp + 8]
0x0017a27f: mov      dword ptr [eax], edx
0x0017a281: mov      dword ptr [edi], ecx
0x0017a283: mov      edx, 0x64
0x0017a288: mov      ecx, 1
0x0017a28d: call     0x10c7c0
0x0017a292: xor      ecx, ecx
0x0017a294: cmp      eax, 0x33
0x0017a297: setl     cl
0x0017a29a: push     ecx
0x0017a29b: push     0x6836c8  ; 'gamselb%d.pcx'
0x0017a2a0: push     0x697428
0x0017a2a5: call     0x2179de
0x0017a2aa: push     0x34
0x0017a2ac: call     0x217492
0x0017a2b1: add      esp, 0x10
0x0017a2b4: mov      dword ptr [ebp + 8], eax
0x0017a2b7: test     eax, eax
0x0017a2b9: mov      byte ptr [ebp - 4], 0xb
0x0017a2bd: je       0x17a2e4
0x0017a2bf: push     0x800
0x0017a2c4: push     0x697428
0x0017a2c9: push     0x64
0x0017a2cb: push     0x258
0x0017a2d0: push     0x320
0x0017a2d5: push     0
0x0017a2d7: push     0
0x0017a2d9: mov      ecx, eax
0x0017a2db: call     0x50340
0x0017a2e0: mov      edi, eax
0x0017a2e2: jmp      0x17a2e6
0x0017a2e6: lea      esi, [ebx + 0x30]
0x0017a2e9: lea      edx, [ebp + 8]
0x0017a2ec: mov      byte ptr [ebp - 4], 6
0x0017a2f0: mov      dword ptr [ebp + 8], edi
0x0017a2f3: mov      eax, dword ptr [esi + 8]
0x0017a2f6: push     edx
0x0017a2f7: push     1
0x0017a2f9: push     eax
0x0017a2fa: mov      ecx, esi
0x0017a2fc: call     0x1fe2d0
0x0017a301: movsx    edx, word ptr [edi + 0x1a]
0x0017a305: mov      eax, dword ptr [ebx + 0x1c]
0x0017a308: mov      ecx, dword ptr [edi + 0x30]
0x0017a30b: add      edx, eax
0x0017a30d: push     0
0x0017a30f: movsx    eax, word ptr [edi + 0x18]
0x0017a313: add      eax, dword ptr [ebx + 0x18]
0x0017a316: mov      dword ptr [ebp - 0x24], eax
0x0017a319: mov      eax, dword ptr [0x6992d0]
0x0017a31e: mov      eax, dword ptr [eax + 0x40]
0x0017a321: mov      edi, dword ptr [eax + 0x2c]
0x0017a324: mov      dword ptr [ebp + 8], edi
0x0017a327: mov      edi, dword ptr [eax + 0x28]
0x0017a32a: mov      dword ptr [ebp - 0x18], edi
0x0017a32d: mov      edi, dword ptr [eax + 0x24]
0x0017a330: mov      dword ptr [ebp - 0x14], edi
0x0017a333: mov      edi, dword ptr [ebp + 8]
0x0017a336: push     edi
0x0017a337: mov      edi, dword ptr [ebp - 0x18]
0x0017a33a: mov      eax, dword ptr [eax + 0x30]
0x0017a33d: push     edi
0x0017a33e: mov      edi, dword ptr [ebp - 0x14]
0x0017a341: push     edi
0x0017a342: push     edx
0x0017a343: mov      edx, dword ptr [ebp - 0x24]
0x0017a346: push     edx
0x0017a347: push     eax
0x0017a348: mov      eax, dword ptr [ecx + 0x28]
0x0017a34b: push     eax
0x0017a34c: mov      eax, dword ptr [ecx + 0x24]
0x0017a34f: push     eax
0x0017a350: push     0
0x0017a352: push     0
0x0017a354: call     0x4df80
0x0017a359: push     0x34
0x0017a35b: call     0x217492
0x0017a360: add      esp, 4
0x0017a363: mov      dword ptr [ebp + 8], eax
0x0017a366: test     eax, eax
0x0017a368: mov      byte ptr [ebp - 4], 0xc
0x0017a36c: je       0x17a396
0x0017a36e: push     0x800
0x0017a373: push     0x68324c  ; 'GSelPop1.pcx'
0x0017a378: push     0x64
0x0017a37a: push     0x249
0x0017a37f: push     0x172
0x0017a384: push     6
0x0017a386: push     0x18c
0x0017a38b: mov      ecx, eax
0x0017a38d: call     0x4ffa0
0x0017a392: mov      edi, eax
0x0017a394: jmp      0x17a398
0x0017a398: lea      ecx, [ebp + 8]
0x0017a39b: mov      byte ptr [ebp - 4], 6
0x0017a39f: mov      dword ptr [ebp + 8], edi
0x0017a3a2: mov      eax, dword ptr [esi + 8]
0x0017a3a5: push     ecx
0x0017a3a6: push     1
0x0017a3a8: push     eax
0x0017a3a9: mov      ecx, esi
0x0017a3ab: call     0x1fe2d0
0x0017a3b0: movsx    edx, word ptr [edi + 0x1a]
0x0017a3b4: add      edx, dword ptr [ebx + 0x1c]
0x0017a3b7: mov      ecx, dword ptr [edi + 0x30]
0x0017a3ba: push     0
0x0017a3bc: push     edx
0x0017a3bd: mov      eax, dword ptr [ecx + 0x28]
0x0017a3c0: movsx    edx, word ptr [edi + 0x18]
0x0017a3c4: add      edx, dword ptr [ebx + 0x18]
0x0017a3c7: push     edx
0x0017a3c8: mov      edx, dword ptr [0x6992d0]
0x0017a3ce: mov      edx, dword ptr [edx + 0x40]
0x0017a3d1: push     edx
0x0017a3d2: push     eax
0x0017a3d3: mov      eax, dword ptr [ecx + 0x24]
0x0017a3d6: push     eax
0x0017a3d7: push     0
0x0017a3d9: push     0
0x0017a3db: call     0x4fa80
0x0017a3e0: push     0x34
0x0017a3e2: call     0x217492
0x0017a3e7: add      esp, 4
0x0017a3ea: mov      dword ptr [ebp + 8], eax
0x0017a3ed: test     eax, eax
0x0017a3ef: mov      byte ptr [ebp - 4], 0xd
0x0017a3f3: je       0x17a41a
0x0017a3f5: push     0x800
0x0017a3fa: push     0x6836b8  ; 'SCSelBck.pcx'
0x0017a3ff: push     0x65
0x0017a401: push     0x249
0x0017a406: push     0x23f
0x0017a40b: push     6
0x0017a40d: push     3
0x0017a40f: mov      ecx, eax
0x0017a411: call     0x4ffa0
0x0017a416: mov      edi, eax
0x0017a418: jmp      0x17a41c
0x0017a41c: push     6
0x0017a41e: push     6
0x0017a420: mov      ecx, edi
0x0017a422: mov      byte ptr [ebp - 4], 6
0x0017a426: call     0x1fed80
0x0017a42b: lea      ecx, [ebp + 8]
0x0017a42e: mov      dword ptr [ebp + 8], edi
0x0017a431: mov      eax, dword ptr [esi + 8]
0x0017a434: push     ecx
0x0017a435: push     1
0x0017a437: push     eax
0x0017a438: mov      ecx, esi
0x0017a43a: call     0x1fe2d0
0x0017a43f: push     0x50
0x0017a441: call     0x217492
0x0017a446: add      esp, 4
0x0017a449: mov      dword ptr [ebp - 0x10], eax
0x0017a44c: test     eax, eax
0x0017a44e: mov      byte ptr [ebp - 4], 0xe
0x0017a452: je       0x17a49a
0x0017a454: mov      cl, byte ptr [ebx + 0x65]
0x0017a457: test     cl, cl
0x0017a459: je       0x17a462
0x0017a45b: mov      ecx, 2
0x0017a460: jmp      0x17a46c
0x0017a46c: mov      edx, dword ptr [ecx*4 + 0x6a8100]
0x0017a473: push     8
0x0017a475: push     0
0x0017a477: push     5
0x0017a479: push     0x169
0x0017a47e: push     8
0x0017a480: push     0x65f2ec  ; 'medfont.fnt'
0x0017a485: push     edx
0x0017a486: push     0x17
0x0017a488: push     0x16f
0x0017a48d: push     0x17
0x0017a48f: push     0x19
0x0017a491: mov      ecx, eax
0x0017a493: call     0x1bc6a0
0x0017a498: jmp      0x17a49c
0x0017a49c: lea      ecx, [ebp + 8]
0x0017a49f: mov      byte ptr [ebp - 4], 6
0x0017a4a3: mov      dword ptr [ebp + 8], eax
0x0017a4a6: mov      eax, dword ptr [esi + 8]
0x0017a4a9: push     ecx
0x0017a4aa: push     1
0x0017a4ac: push     eax
0x0017a4ad: mov      ecx, esi
0x0017a4af: call     0x1fe2d0
0x0017a4b4: mov      al, byte ptr [ebx + 0x64]
0x0017a4b7: test     al, al
0x0017a4b9: jne      0x17a4ca
0x0017a4bb: mov      cl, byte ptr [ebx + 0x65]
0x0017a4be: test     cl, cl
0x0017a4c0: je       0x17a4e0
0x0017a4c2: test     al, al
0x0017a4c4: je       0x17a59e
0x0017a4ca: mov      eax, dword ptr [0x69959c]
0x0017a4cf: test     eax, eax
0x0017a4d1: jne      0x17a4e0
0x0017a4d3: cmp      dword ptr [0x698a40], 3
0x0017a4da: jne      0x17a59e
0x0017a4e0: push     0x34
0x0017a4e2: call     0x217492
0x0017a4e7: add      esp, 4
0x0017a4ea: mov      dword ptr [ebp + 8], eax
0x0017a4ed: test     eax, eax
0x0017a4ef: mov      byte ptr [ebp - 4], 0xf
0x0017a4f3: je       0x17a51a
0x0017a4f5: push     0x800
0x0017a4fa: push     0x68323c  ; 'AdvOptBk.pcx'
0x0017a4ff: push     0x66
0x0017a501: push     0x249
0x0017a506: push     0x22d
0x0017a50b: push     6
0x0017a50d: push     3
0x0017a50f: mov      ecx, eax
0x0017a511: call     0x4ffa0
0x0017a516: mov      edi, eax
0x0017a518: jmp      0x17a51c
0x0017a51c: push     6
0x0017a51e: push     6
0x0017a520: mov      ecx, edi
0x0017a522: mov      byte ptr [ebp - 4], 6
0x0017a526: call     0x1fed80
0x0017a52b: lea      edx, [ebp + 8]
0x0017a52e: mov      dword ptr [ebp + 8], edi
0x0017a531: mov      eax, dword ptr [esi + 8]
0x0017a534: push     edx
0x0017a535: push     1
0x0017a537: push     eax
0x0017a538: mov      ecx, esi
0x0017a53a: call     0x1fe2d0
0x0017a53f: push     0x34
0x0017a541: call     0x217492
0x0017a546: add      esp, 4
0x0017a549: mov      dword ptr [ebp + 8], eax
0x0017a54c: test     eax, eax
0x0017a54e: mov      byte ptr [ebp - 4], 0x10
0x0017a552: je       0x17a579
0x0017a554: push     0x800
0x0017a559: push     0x6836a8  ; 'RanMapBk.pcx'
0x0017a55e: push     0x67
0x0017a560: push     0x249
0x0017a565: push     0x22d
0x0017a56a: push     6
0x0017a56c: push     3
0x0017a56e: mov      ecx, eax
0x0017a570: call     0x4ffa0
0x0017a575: mov      edi, eax
0x0017a577: jmp      0x17a57b
0x0017a57b: push     6
0x0017a57d: push     6
0x0017a57f: mov      ecx, edi
0x0017a581: mov      byte ptr [ebp - 4], 6
0x0017a585: call     0x1fed80
0x0017a58a: lea      ecx, [ebp + 8]
0x0017a58d: mov      dword ptr [ebp + 8], edi
0x0017a590: mov      eax, dword ptr [esi + 8]
0x0017a593: push     ecx
0x0017a594: push     1
0x0017a596: push     eax
0x0017a597: mov      ecx, esi
0x0017a599: call     0x1fe2d0
0x0017a59e: mov      edx, dword ptr [0x6a5dc4]
0x0017a5a4: mov      eax, dword ptr [edx + 0x20]
0x0017a5a7: mov      eax, dword ptr [eax + 0x7b4]
0x0017a5ad: push     eax
0x0017a5ae: push     0x660d28
0x0017a5b3: push     0x697428
0x0017a5b8: call     0x2179de
0x0017a5bd: push     0x50
0x0017a5bf: call     0x217492
0x0017a5c4: add      esp, 0x10
0x0017a5c7: mov      dword ptr [ebp - 0x10], eax
0x0017a5ca: test     eax, eax
0x0017a5cc: mov      byte ptr [ebp - 4], 0x11
0x0017a5d0: je       0x17a603
0x0017a5d2: push     8
0x0017a5d4: push     0
0x0017a5d6: push     5
0x0017a5d8: push     0x84
0x0017a5dd: push     2
0x0017a5df: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a5e4: push     0x697428
0x0017a5e9: push     0x13
0x0017a5eb: push     0x14e
0x0017a5f0: push     0x1b3
0x0017a5f5: push     0x19e
0x0017a5fa: mov      ecx, eax
0x0017a5fc: call     0x1bc6a0
0x0017a601: jmp      0x17a605
0x0017a605: lea      ecx, [ebp + 8]
0x0017a608: mov      byte ptr [ebp - 4], 6
0x0017a60c: mov      dword ptr [ebp + 8], eax
0x0017a60f: mov      eax, dword ptr [esi + 8]
0x0017a612: push     ecx
0x0017a613: push     1
0x0017a615: push     eax
0x0017a616: mov      ecx, esi
0x0017a618: call     0x1fe2d0
0x0017a61d: mov      edx, dword ptr [0x6a5dc4]
0x0017a623: mov      eax, dword ptr [edx + 0x20]
0x0017a626: mov      eax, dword ptr [eax + 0x36c]
0x0017a62c: push     eax
0x0017a62d: push     0x660d28
0x0017a632: push     0x697428
0x0017a637: call     0x2179de
0x0017a63c: push     0x50
0x0017a63e: call     0x217492
0x0017a643: add      esp, 0x10
0x0017a646: mov      dword ptr [ebp - 0x10], eax
0x0017a649: test     eax, eax
0x0017a64b: mov      byte ptr [ebp - 4], 0x12
0x0017a64f: je       0x17a67f
0x0017a651: push     8
0x0017a653: push     0
0x0017a655: push     5
0x0017a657: push     0x85
0x0017a65c: push     2
0x0017a65e: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a663: push     0x697428
0x0017a668: push     0x13
0x0017a66a: push     0x54
0x0017a66c: push     0x1b3
0x0017a671: push     0x299
0x0017a676: mov      ecx, eax
0x0017a678: call     0x1bc6a0
0x0017a67d: jmp      0x17a681
0x0017a681: lea      ecx, [ebp + 8]
0x0017a684: mov      byte ptr [ebp - 4], 6
0x0017a688: mov      dword ptr [ebp + 8], eax
0x0017a68b: mov      eax, dword ptr [esi + 8]
0x0017a68e: push     ecx
0x0017a68f: push     1
0x0017a691: push     eax
0x0017a692: mov      ecx, esi
0x0017a694: call     0x1fe2d0
0x0017a699: push     0x50
0x0017a69b: call     0x217492
0x0017a6a0: add      esp, 4
0x0017a6a3: mov      dword ptr [ebp - 0x10], eax
0x0017a6a6: test     eax, eax
0x0017a6a8: mov      byte ptr [ebp - 4], 0x13
0x0017a6ac: je       0x17a6e7
0x0017a6ae: mov      edx, dword ptr [0x6a5dc4]
0x0017a6b4: push     8
0x0017a6b6: push     0
0x0017a6b8: push     5
0x0017a6ba: mov      ecx, dword ptr [edx + 0x20]
0x0017a6bd: push     0x86
0x0017a6c2: push     2
0x0017a6c4: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a6c9: mov      ecx, dword ptr [ecx + 0x7bc]
0x0017a6cf: push     ecx
0x0017a6d0: push     0x13
0x0017a6d2: push     0x5a
0x0017a6d4: push     0x1b3
0x0017a6d9: push     0x19e
0x0017a6de: mov      ecx, eax
0x0017a6e0: call     0x1bc6a0
0x0017a6e5: jmp      0x17a6e9
0x0017a6e9: lea      ecx, [ebp + 8]
0x0017a6ec: mov      byte ptr [ebp - 4], 6
0x0017a6f0: mov      dword ptr [ebp + 8], eax
0x0017a6f3: mov      eax, dword ptr [esi + 8]
0x0017a6f6: push     ecx
0x0017a6f7: push     1
0x0017a6f9: push     eax
0x0017a6fa: mov      ecx, esi
0x0017a6fc: call     0x1fe2d0
0x0017a701: push     0x50
0x0017a703: call     0x217492
0x0017a708: add      esp, 4
0x0017a70b: mov      dword ptr [ebp - 0x10], eax
0x0017a70e: test     eax, eax
0x0017a710: mov      byte ptr [ebp - 4], 0x14
0x0017a714: je       0x17a74c
0x0017a716: mov      edx, dword ptr [0x6a5dc4]
0x0017a71c: push     8
0x0017a71e: push     0
0x0017a720: push     4
0x0017a722: mov      ecx, dword ptr [edx + 0x20]
0x0017a725: push     0x64
0x0017a727: push     2
0x0017a729: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a72e: mov      ecx, dword ptr [ecx + 0x7c0]
0x0017a734: push     ecx
0x0017a735: push     0x12
0x0017a737: push     0x116
0x0017a73c: push     0x1b
0x0017a73e: push     0x1a6
0x0017a743: mov      ecx, eax
0x0017a745: call     0x1bc6a0
0x0017a74a: jmp      0x17a74e
0x0017a74e: lea      ecx, [ebp + 8]
0x0017a751: mov      byte ptr [ebp - 4], 6
0x0017a755: mov      dword ptr [ebp + 8], eax
0x0017a758: mov      eax, dword ptr [esi + 8]
0x0017a75b: push     ecx
0x0017a75c: push     1
0x0017a75e: push     eax
0x0017a75f: mov      ecx, esi
0x0017a761: call     0x1fe2d0
0x0017a766: push     0x50
0x0017a768: call     0x217492
0x0017a76d: add      esp, 4
0x0017a770: mov      dword ptr [ebp - 0x10], eax
0x0017a773: test     eax, eax
0x0017a775: mov      byte ptr [ebp - 4], 0x15
0x0017a779: je       0x17a7b4
0x0017a77b: mov      edx, dword ptr [0x6a5dc4]
0x0017a781: push     8
0x0017a783: push     0
0x0017a785: push     4
0x0017a787: mov      ecx, dword ptr [edx + 0x20]
0x0017a78a: push     0x69
0x0017a78c: push     2
0x0017a78e: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a793: mov      ecx, dword ptr [ecx + 0x7c4]
0x0017a799: push     ecx
0x0017a79a: push     0x12
0x0017a79c: push     0x116
0x0017a7a1: push     0x89
0x0017a7a6: push     0x1a6
0x0017a7ab: mov      ecx, eax
0x0017a7ad: call     0x1bc6a0
0x0017a7b2: jmp      0x17a7b6
0x0017a7b6: lea      ecx, [ebp + 8]
0x0017a7b9: mov      byte ptr [ebp - 4], 6
0x0017a7bd: mov      dword ptr [ebp + 8], eax
0x0017a7c0: mov      eax, dword ptr [esi + 8]
0x0017a7c3: push     ecx
0x0017a7c4: push     1
0x0017a7c6: push     eax
0x0017a7c7: mov      ecx, esi
0x0017a7c9: call     0x1fe2d0
0x0017a7ce: push     0x5c
0x0017a7d0: call     0x217492
0x0017a7d5: add      esp, 4
0x0017a7d8: mov      dword ptr [ebp + 8], eax
0x0017a7db: test     eax, eax
0x0017a7dd: mov      byte ptr [ebp - 4], 0x16
0x0017a7e1: je       0x17a80b
0x0017a7e3: push     1
0x0017a7e5: push     4
0x0017a7e7: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a7ec: push     0x73
0x0017a7ee: push     0x13f
0x0017a7f3: push     0x9b
0x0017a7f8: push     0x1a6
0x0017a7fd: push     0x691260
0x0017a802: mov      ecx, eax
0x0017a804: call     0x1ba360
0x0017a809: jmp      0x17a80d
0x0017a80d: mov      byte ptr [ebp - 4], 6
0x0017a811: mov      edx, eax
0x0017a813: lea      esi, [ebx + 0x30]
0x0017a816: mov      dword ptr [ebx + 0x196c], eax
0x0017a81c: lea      ecx, [ebp + 8]
0x0017a81f: mov      dword ptr [ebp + 8], edx
0x0017a822: mov      eax, dword ptr [esi + 8]
0x0017a825: push     ecx
0x0017a826: push     1
0x0017a828: push     eax
0x0017a829: mov      ecx, esi
0x0017a82b: call     0x1fe2d0
0x0017a830: push     0x50
0x0017a832: call     0x217492
0x0017a837: add      esp, 4
0x0017a83a: mov      dword ptr [ebp - 0x10], eax
0x0017a83d: test     eax, eax
0x0017a83f: mov      byte ptr [ebp - 4], 0x17
0x0017a843: je       0x17a87e
0x0017a845: mov      edx, dword ptr [0x6a5dc4]
0x0017a84b: push     8
0x0017a84d: push     0
0x0017a84f: push     4
0x0017a851: mov      ecx, dword ptr [edx + 0x20]
0x0017a854: push     0x64
0x0017a856: push     2
0x0017a858: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a85d: mov      ecx, dword ptr [ecx + 0x7c8]
0x0017a863: push     ecx
0x0017a864: push     0x12
0x0017a866: push     0x116
0x0017a86b: push     0x120
0x0017a870: push     0x1a6
0x0017a875: mov      ecx, eax
0x0017a877: call     0x1bc6a0
0x0017a87c: jmp      0x17a880
0x0017a880: lea      ecx, [ebp + 8]
0x0017a883: mov      byte ptr [ebp - 4], 6
0x0017a887: mov      dword ptr [ebp + 8], eax
0x0017a88a: mov      eax, dword ptr [esi + 8]
0x0017a88d: push     ecx
0x0017a88e: push     1
0x0017a890: push     eax
0x0017a891: mov      ecx, esi
0x0017a893: call     0x1fe2d0
0x0017a898: push     0x50
0x0017a89a: call     0x217492
0x0017a89f: add      esp, 4
0x0017a8a2: mov      dword ptr [ebp - 0x10], eax
0x0017a8a5: test     eax, eax
0x0017a8a7: mov      byte ptr [ebp - 4], 0x18
0x0017a8ab: je       0x17a8e6
0x0017a8ad: mov      edx, dword ptr [0x6a5dc4]
0x0017a8b3: push     8
0x0017a8b5: push     0
0x0017a8b7: push     4
0x0017a8b9: mov      ecx, dword ptr [edx + 0x20]
0x0017a8bc: push     0x64
0x0017a8be: push     2
0x0017a8c0: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a8c5: mov      ecx, dword ptr [ecx + 0x7cc]
0x0017a8cb: push     ecx
0x0017a8cc: push     0x12
0x0017a8ce: push     0x116
0x0017a8d3: push     0x158
0x0017a8d8: push     0x1a6
0x0017a8dd: mov      ecx, eax
0x0017a8df: call     0x1bc6a0
0x0017a8e4: jmp      0x17a8e8
0x0017a8e8: lea      ecx, [ebp + 8]
0x0017a8eb: mov      byte ptr [ebp - 4], 6
0x0017a8ef: mov      dword ptr [ebp + 8], eax
0x0017a8f2: mov      eax, dword ptr [esi + 8]
0x0017a8f5: push     ecx
0x0017a8f6: push     1
0x0017a8f8: push     eax
0x0017a8f9: mov      ecx, esi
0x0017a8fb: call     0x1fe2d0
0x0017a900: mov      eax, dword ptr [0x69959c]
0x0017a905: test     eax, eax
0x0017a907: je       0x17a981
0x0017a909: mov      al, byte ptr [ebx + 0x65]
0x0017a90c: test     al, al
0x0017a90e: jne      0x17a981
0x0017a910: push     0x70
0x0017a912: call     0x217492
0x0017a917: add      esp, 4
0x0017a91a: mov      dword ptr [ebp + 8], eax
0x0017a91d: test     eax, eax
0x0017a91f: mov      byte ptr [ebp - 4], 0x19
0x0017a923: je       0x17a95c
0x0017a925: push     4
0x0017a927: push     2
0x0017a929: push     0xf
0x0017a92b: push     0
0x0017a92d: push     1
0x0017a92f: push     0
0x0017a931: push     0x65f2f8  ; 'smalfont.fnt'
0x0017a936: push     0x691260
0x0017a93b: push     0x68369c  ; 'gspbut2.def'
0x0017a940: push     0x83
0x0017a945: push     0x14
0x0017a947: push     0x80
0x0017a94c: push     0x51
0x0017a94e: push     0x26e
0x0017a953: mov      ecx, eax
0x0017a955: call     0x56730
0x0017a95a: jmp      0x17a95e
0x0017a95e: mov      byte ptr [ebp - 4], 6
0x0017a962: mov      edx, eax
0x0017a964: lea      esi, [ebx + 0x30]
0x0017a967: mov      dword ptr [ebx + 0x1868], eax
0x0017a96d: lea      ecx, [ebp + 8]
0x0017a970: mov      dword ptr [ebp + 8], edx
0x0017a973: mov      eax, dword ptr [esi + 8]
0x0017a976: push     ecx
0x0017a977: push     1
0x0017a979: push     eax
0x0017a97a: mov      ecx, esi
0x0017a97c: call     0x1fe2d0
0x0017a981: push     0x48
0x0017a983: call     0x217492
0x0017a988: add      esp, 4
0x0017a98b: mov      dword ptr [ebp - 0x10], eax
0x0017a98e: test     eax, eax
0x0017a990: mov      byte ptr [ebp - 4], 0x1a
0x0017a994: je       0x17a9be
0x0017a996: push     0x10
0x0017a998: push     0
0x0017a99a: push     0
0x0017a99c: push     0
0x0017a99e: push     0
0x0017a9a0: push     0x660e68  ; 'scnrmpsz.def'
0x0017a9a5: push     0xbd
0x0017a9aa: push     0x17
0x0017a9ac: push     0x1d
0x0017a9ae: push     0x1c
0x0017a9b0: push     0x2ca
0x0017a9b5: mov      ecx, eax
0x0017a9b7: call     0xea800
0x0017a9bc: jmp      0x17a9c0
0x0017a9c0: lea      edx, [ebp + 8]
0x0017a9c3: mov      byte ptr [ebp - 4], 6
0x0017a9c7: mov      dword ptr [ebp + 8], eax
0x0017a9ca: mov      eax, dword ptr [esi + 8]
0x0017a9cd: push     edx
0x0017a9ce: push     1
0x0017a9d0: push     eax
0x0017a9d1: mov      ecx, esi
0x0017a9d3: call     0x1fe2d0
0x0017a9d8: mov      eax, dword ptr [0x6a5dc4]
0x0017a9dd: mov      eax, dword ptr [eax + 0x20]
0x0017a9e0: mov      eax, dword ptr [eax + 0x61c]
0x0017a9e6: push     eax
0x0017a9e7: push     0x660d28
0x0017a9ec: push     0x697428
0x0017a9f1: call     0x2179de
0x0017a9f6: push     0x50
0x0017a9f8: call     0x217492
0x0017a9fd: add      esp, 0x10
0x0017aa00: mov      dword ptr [ebp - 0x10], eax
0x0017aa03: test     eax, eax
0x0017aa05: mov      byte ptr [ebp - 4], 0x1b
0x0017aa09: je       0x17aa36
0x0017aa0b: push     8
0x0017aa0d: push     0
0x0017aa0f: push     6
0x0017aa11: push     0x64
0x0017aa13: push     4
0x0017aa15: push     0x65f2f8  ; 'smalfont.fnt'
0x0017aa1a: push     0x697428
0x0017aa1f: push     0x17
0x0017aa21: push     0x2c
0x0017aa23: push     0x193
0x0017aa28: push     0x19e
0x0017aa2d: mov      ecx, eax
0x0017aa2f: call     0x1bc6a0
0x0017aa34: jmp      0x17aa38
0x0017aa38: lea      ecx, [ebp + 8]
0x0017aa3b: mov      byte ptr [ebp - 4], 6
0x0017aa3f: mov      dword ptr [ebp + 8], eax
0x0017aa42: mov      eax, dword ptr [esi + 8]
0x0017aa45: push     ecx
0x0017aa46: push     1
0x0017aa48: push     eax
0x0017aa49: mov      ecx, esi
0x0017aa4b: call     0x1fe2d0
0x0017aa50: mov      edx, dword ptr [0x6a5dc4]
0x0017aa56: mov      eax, dword ptr [edx + 0x20]
0x0017aa59: mov      eax, dword ptr [eax + 0x620]
0x0017aa5f: push     eax
0x0017aa60: push     0x660d28
0x0017aa65: push     0x697428
0x0017aa6a: call     0x2179de
0x0017aa6f: push     0x50
0x0017aa71: call     0x217492
0x0017aa76: add      esp, 0x10
0x0017aa79: mov      dword ptr [ebp - 0x10], eax
0x0017aa7c: test     eax, eax
0x0017aa7e: mov      byte ptr [ebp - 4], 0x1c
0x0017aa82: je       0x17aab2
0x0017aa84: push     8
0x0017aa86: push     0
0x0017aa88: push     6
0x0017aa8a: push     0x182
0x0017aa8f: push     4
0x0017aa91: push     0x65f2f8  ; 'smalfont.fnt'
0x0017aa96: push     0x697428
0x0017aa9b: push     0x17
0x0017aa9d: push     0x3a
0x0017aa9f: push     0x193
0x0017aaa4: push     0x243
0x0017aaa9: mov      ecx, eax
0x0017aaab: call     0x1bc6a0
0x0017aab0: jmp      0x17aab4
0x0017aab4: lea      ecx, [ebp - 0x18]
0x0017aab7: mov      byte ptr [ebp - 4], 6
0x0017aabb: mov      dword ptr [ebp - 0x18], eax
0x0017aabe: mov      eax, dword ptr [esi + 8]
0x0017aac1: push     ecx
0x0017aac2: push     1
0x0017aac4: push     eax
0x0017aac5: mov      ecx, esi
0x0017aac7: call     0x1fe2d0
0x0017aacc: mov      al, byte ptr [ebx + 0x65]
0x0017aacf: test     al, al
0x0017aad1: jne      0x17aaec
0x0017aad3: mov      al, byte ptr [ebx + 0x64]
0x0017aad6: test     al, al
0x0017aad8: je       0x17ab04
0x0017aada: mov      eax, dword ptr [0x69959c]
0x0017aadf: test     eax, eax
0x0017aae1: jne      0x17ab04
0x0017aae3: cmp      dword ptr [0x698a40], 3
0x0017aaea: je       0x17ab04
0x0017aaec: mov      ecx, dword ptr [ebp + 8]
0x0017aaef: push     6
0x0017aaf1: push     6
0x0017aaf3: call     0x1fed80
0x0017aaf8: mov      ecx, dword ptr [ebp - 0x18]
0x0017aafb: push     6
0x0017aafd: push     6
0x0017aaff: call     0x1fed80
0x0017ab04: mov      edi, 0x78
0x0017ab09: mov      dword ptr [ebp + 8], 0x280
0x0017ab10: push     0x48
0x0017ab12: call     0x217492
0x0017ab17: add      esp, 4
0x0017ab1a: mov      dword ptr [ebp - 0x10], eax
0x0017ab1d: test     eax, eax
0x0017ab1f: mov      byte ptr [ebp - 4], 0x1d
0x0017ab23: je       0x17ab54
0x0017ab25: mov      ecx, dword ptr [ebp + 8]
0x0017ab28: push     0x10
0x0017ab2a: push     0
0x0017ab2c: push     0
0x0017ab2e: push     0
0x0017ab30: push     0
0x0017ab32: lea      edx, [edi - 8]
0x0017ab35: push     0x660d18  ; 'itgflags.def'
0x0017ab3a: push     edx
0x0017ab3b: push     0x14
0x0017ab3d: push     0xf
0x0017ab3f: add      ecx, 0xffffff4c
0x0017ab45: push     0x195
0x0017ab4a: push     ecx
0x0017ab4b: mov      ecx, eax
0x0017ab4d: call     0xea800
0x0017ab52: jmp      0x17ab56
0x0017ab56: push     6
0x0017ab58: push     6
0x0017ab5a: mov      ecx, eax
0x0017ab5c: mov      byte ptr [ebp - 4], 6
0x0017ab60: mov      dword ptr [ebp - 0x14], eax
0x0017ab63: call     0x1fed80
0x0017ab68: mov      eax, dword ptr [esi + 8]
0x0017ab6b: lea      edx, [ebp - 0x14]
0x0017ab6e: push     edx
0x0017ab6f: push     1
0x0017ab71: push     eax
0x0017ab72: mov      ecx, esi
0x0017ab74: call     0x1fe2d0
0x0017ab79: push     0x48
0x0017ab7b: call     0x217492
0x0017ab80: add      esp, 4
0x0017ab83: mov      dword ptr [ebp - 0x10], eax
0x0017ab86: test     eax, eax
0x0017ab88: mov      byte ptr [ebp - 4], 0x1e
0x0017ab8c: je       0x17abb4
0x0017ab8e: push     0x10
0x0017ab90: mov      ecx, dword ptr [ebp + 8]
0x0017ab93: push     0
0x0017ab95: push     0
0x0017ab97: push     0
0x0017ab99: push     0
0x0017ab9b: push     0x660d18  ; 'itgflags.def'
0x0017aba0: push     edi
0x0017aba1: push     0x14
0x0017aba3: push     0xf
0x0017aba5: push     0x195
0x0017abaa: push     ecx
0x0017abab: mov      ecx, eax
0x0017abad: call     0xea800
0x0017abb2: jmp      0x17abb6
0x0017abb6: push     6
0x0017abb8: push     6
0x0017abba: mov      ecx, eax
0x0017abbc: mov      byte ptr [ebp - 4], 6
0x0017abc0: mov      dword ptr [ebp - 0x14], eax
0x0017abc3: call     0x1fed80
0x0017abc8: mov      eax, dword ptr [esi + 8]
0x0017abcb: lea      edx, [ebp - 0x14]
0x0017abce: push     edx
0x0017abcf: push     1
0x0017abd1: push     eax
0x0017abd2: mov      ecx, esi
0x0017abd4: call     0x1fe2d0
0x0017abd9: mov      edx, dword ptr [ebp + 8]
0x0017abdc: inc      edi
0x0017abdd: add      edx, 0xf
0x0017abe0: lea      eax, [edi - 0x78]
0x0017abe3: mov      dword ptr [ebp + 8], edx
0x0017abe6: cmp      eax, 8
0x0017abe9: jl       0x17ab10
0x0017abef: mov      al, byte ptr [ebx + 0x65]
0x0017abf2: test     al, al
0x0017abf4: jne      0x17ac5d
0x0017abf6: mov      al, byte ptr [ebx + 0x64]
0x0017abf9: test     al, al
0x0017abfb: je       0x17ac0f
0x0017abfd: mov      eax, dword ptr [0x69959c]
0x0017ac02: test     eax, eax
0x0017ac04: jne      0x17ac0f
0x0017ac06: cmp      dword ptr [0x698a40], 3
0x0017ac0d: jne      0x17ac5d
0x0017ac0f: push     0x30
0x0017ac11: call     0x217492
0x0017ac16: add      esp, 4
0x0017ac19: mov      dword ptr [ebp - 0x10], eax
0x0017ac1c: test     eax, eax
0x0017ac1e: mov      byte ptr [ebp - 4], 0x1f
0x0017ac22: je       0x17ac43
0x0017ac24: push     0x183
0x0017ac29: push     0x19
0x0017ac2b: push     0x136
0x0017ac30: push     0x192
0x0017ac35: push     0x1c8
0x0017ac3a: mov      ecx, eax
0x0017ac3c: call     0x1755c0
0x0017ac41: jmp      0x17ac45
0x0017ac45: lea      ecx, [ebp + 8]
0x0017ac48: mov      byte ptr [ebp - 4], 6
0x0017ac4c: mov      dword ptr [ebp + 8], eax
0x0017ac4f: mov      eax, dword ptr [esi + 8]
0x0017ac52: push     ecx
0x0017ac53: push     1
0x0017ac55: push     eax
0x0017ac56: mov      ecx, esi
0x0017ac58: call     0x1fe2d0
0x0017ac5d: mov      eax, dword ptr [0x69fe24]
0x0017ac62: mov      dword ptr [ebp + 8], 0
0x0017ac69: test     eax, eax
0x0017ac6b: jle      0x17ad01
0x0017ac71: mov      dword ptr [ebp - 0x14], 0x7a
0x0017ac78: push     0x50
0x0017ac7a: call     0x217492
0x0017ac7f: add      esp, 4
0x0017ac82: mov      dword ptr [ebp - 0x10], eax
0x0017ac85: test     eax, eax
0x0017ac87: mov      byte ptr [ebp - 4], 0x20
0x0017ac8b: je       0x17acbe
0x0017ac8d: mov      edx, dword ptr [ebp + 8]
0x0017ac90: mov      ecx, dword ptr [ebp - 0x14]
0x0017ac93: push     8
0x0017ac95: push     0
0x0017ac97: add      edx, 0x8e
0x0017ac9d: push     1
0x0017ac9f: push     edx
0x0017aca0: push     4
0x0017aca2: push     0x65f2f8  ; 'smalfont.fnt'
0x0017aca7: push     0
0x0017aca9: push     0x19
0x0017acab: push     0x13a
0x0017acb0: push     ecx
0x0017acb1: push     0x39
0x0017acb3: mov      ecx, eax
0x0017acb5: call     0x1bc6a0
0x0017acba: mov      edi, eax
0x0017acbc: jmp      0x17acc0
0x0017acc0: push     6
0x0017acc2: push     6
0x0017acc4: mov      ecx, edi
0x0017acc6: mov      byte ptr [ebp - 4], 6
0x0017acca: call     0x1fed80
0x0017accf: lea      edx, [ebp - 0x24]
0x0017acd2: mov      dword ptr [ebp - 0x24], edi
0x0017acd5: mov      eax, dword ptr [esi + 8]
0x0017acd8: push     edx
0x0017acd9: push     1
0x0017acdb: push     eax
0x0017acdc: mov      ecx, esi
0x0017acde: call     0x1fe2d0
0x0017ace3: mov      eax, dword ptr [ebp + 8]
0x0017ace6: mov      edx, dword ptr [ebp - 0x14]
0x0017ace9: mov      ecx, dword ptr [0x69fe24]
0x0017acef: inc      eax
0x0017acf0: add      edx, 0x19
0x0017acf3: cmp      eax, ecx
0x0017acf5: mov      dword ptr [ebp + 8], eax
0x0017acf8: mov      dword ptr [ebp - 0x14], edx
0x0017acfb: jl       0x17ac78
0x0017ad01: mov      al, byte ptr [ebx + 0x65]
0x0017ad04: test     al, al
0x0017ad06: je       0x17adf3
0x0017ad0c: push     0x34
0x0017ad0e: call     0x217492
0x0017ad13: add      esp, 4
0x0017ad16: mov      dword ptr [ebp - 0x10], eax
0x0017ad19: test     eax, eax
0x0017ad1b: mov      byte ptr [ebp - 4], 0x21
0x0017ad1f: je       0x17ad4d
0x0017ad21: mov      edi, dword ptr [ebx + 0x1c]
0x0017ad24: push     0x800
0x0017ad29: push     0x683690  ; 'gsstrip.pcx'
0x0017ad2e: mov      ecx, 0x208
0x0017ad33: push     0x184
0x0017ad38: push     0x41
0x0017ad3a: sub      ecx, edi
0x0017ad3c: push     0x18f
0x0017ad41: push     ecx
0x0017ad42: push     3
0x0017ad44: mov      ecx, eax
0x0017ad46: call     0x4ffa0
0x0017ad4b: jmp      0x17ad4f
0x0017ad4f: lea      edx, [ebp + 8]
0x0017ad52: mov      byte ptr [ebp - 4], 6
0x0017ad56: mov      dword ptr [ebp + 8], eax
0x0017ad59: mov      eax, dword ptr [esi + 8]
0x0017ad5c: push     edx
0x0017ad5d: push     1
0x0017ad5f: push     eax
0x0017ad60: mov      ecx, esi
0x0017ad62: call     0x1fe2d0
0x0017ad67: push     0x6603ec
0x0017ad6c: push     0x68338c  ; 'NEWGAME.gm1'
0x0017ad71: call     0x217fbb
0x0017ad76: push     0x70
0x0017ad78: call     0x217492
0x0017ad7d: mov      esi, eax
0x0017ad7f: add      esp, 0xc
0x0017ad82: mov      dword ptr [ebp + 8], esi
0x0017ad85: test     esi, esi
0x0017ad87: mov      byte ptr [ebp - 4], 0x22
0x0017ad8b: je       0x17adce
0x0017ad8d: push     5
0x0017ad8f: push     7
0x0017ad91: push     0
0x0017ad93: push     0x100
0x0017ad98: push     0xa0
0x0017ad9d: push     0
0x0017ad9f: push     0
0x0017ada1: push     4
0x0017ada3: push     4
0x0017ada5: push     0x65f2f8  ; 'smalfont.fnt'
0x0017adaa: push     0x68338c  ; 'NEWGAME.gm1'
0x0017adaf: push     0x3d
0x0017adb1: push     0x15
0x0017adb3: push     0x156
0x0017adb8: push     0x21e
0x0017adbd: push     0x23
0x0017adbf: mov      ecx, esi
0x0017adc1: call     0x1bacd0
0x0017adc6: mov      dword ptr [esi], 0x641c70
0x0017adcc: jmp      0x17add0
0x0017add0: mov      byte ptr [ebp - 4], 6
0x0017add4: mov      dword ptr [ebx + 0x380], esi
0x0017adda: mov      eax, esi
0x0017addc: lea      esi, [ebx + 0x30]
0x0017addf: lea      ecx, [ebp + 8]
0x0017ade2: mov      dword ptr [ebp + 8], eax
0x0017ade5: mov      eax, dword ptr [esi + 8]
0x0017ade8: push     ecx
0x0017ade9: push     1
0x0017adeb: push     eax
0x0017adec: mov      ecx, esi
0x0017adee: call     0x1fe2d0
0x0017adf3: push     0x68
0x0017adf5: call     0x217492
0x0017adfa: add      esp, 4
0x0017adfd: mov      dword ptr [ebp - 0x10], eax
0x0017ae00: test     eax, eax
0x0017ae02: mov      byte ptr [ebp - 4], 0x23
0x0017ae06: je       0x17ae30
0x0017ae08: push     2
0x0017ae0a: push     0
0x0017ae0c: push     0
0x0017ae0e: push     1
0x0017ae10: push     0
0x0017ae12: push     0x683684  ; 'scsmbut.def'
0x0017ae17: push     0x89
0x0017ae1c: push     0x21
0x0017ae1e: push     0x2c
0x0017ae20: push     0x34
0x0017ae22: push     0xa1
0x0017ae27: mov      ecx, eax
0x0017ae29: call     0x55bd0
0x0017ae2e: jmp      0x17ae32
0x0017ae32: lea      edx, [ebp + 8]
0x0017ae35: mov      byte ptr [ebp - 4], 6
0x0017ae39: mov      dword ptr [ebp + 8], eax
0x0017ae3c: mov      eax, dword ptr [esi + 8]
0x0017ae3f: push     edx
0x0017ae40: push     1
0x0017ae42: push     eax
0x0017ae43: mov      ecx, esi
0x0017ae45: call     0x1fe2d0
0x0017ae4a: push     0x68
0x0017ae4c: call     0x217492
0x0017ae51: add      esp, 4
0x0017ae54: mov      dword ptr [ebp - 0x10], eax
0x0017ae57: test     eax, eax
0x0017ae59: mov      byte ptr [ebp - 4], 0x24
0x0017ae5d: je       0x17ae87
0x0017ae5f: push     2
0x0017ae61: push     0
0x0017ae63: push     0
0x0017ae65: push     1
0x0017ae67: push     0
0x0017ae69: push     0x683678  ; 'scmdbut.def'
0x0017ae6e: push     0x8a
0x0017ae73: push     0x21
0x0017ae75: push     0x2c
0x0017ae77: push     0x34
0x0017ae79: push     0xd0
0x0017ae7e: mov      ecx, eax
0x0017ae80: call     0x55bd0
0x0017ae85: jmp      0x17ae89
0x0017ae89: lea      ecx, [ebp + 8]
0x0017ae8c: mov      byte ptr [ebp - 4], 6
0x0017ae90: mov      dword ptr [ebp + 8], eax
0x0017ae93: mov      eax, dword ptr [esi + 8]
0x0017ae96: push     ecx
0x0017ae97: push     1
0x0017ae99: push     eax
0x0017ae9a: mov      ecx, esi
0x0017ae9c: call     0x1fe2d0
0x0017aea1: push     0x68
0x0017aea3: call     0x217492
0x0017aea8: add      esp, 4
0x0017aeab: mov      dword ptr [ebp - 0x10], eax
0x0017aeae: test     eax, eax
0x0017aeb0: mov      byte ptr [ebp - 4], 0x25
0x0017aeb4: je       0x17aede
0x0017aeb6: push     2
0x0017aeb8: push     0
0x0017aeba: push     0
0x0017aebc: push     1
0x0017aebe: push     0
0x0017aec0: push     0x68366c  ; 'sclgbut.def'
0x0017aec5: push     0x8b
0x0017aeca: push     0x21
0x0017aecc: push     0x2c
0x0017aece: push     0x34
0x0017aed0: push     0xff
0x0017aed5: mov      ecx, eax
0x0017aed7: call     0x55bd0
0x0017aedc: jmp      0x17aee0
0x0017aee0: lea      edx, [ebp + 8]
0x0017aee3: mov      byte ptr [ebp - 4], 6
0x0017aee7: mov      dword ptr [ebp + 8], eax
0x0017aeea: mov      eax, dword ptr [esi + 8]
0x0017aeed: push     edx
0x0017aeee: push     1
0x0017aef0: push     eax
0x0017aef1: mov      ecx, esi
0x0017aef3: call     0x1fe2d0
0x0017aef8: push     0x68
0x0017aefa: call     0x217492
0x0017aeff: add      esp, 4
0x0017af02: mov      dword ptr [ebp - 0x10], eax
0x0017af05: test     eax, eax
0x0017af07: mov      byte ptr [ebp - 4], 0x26
0x0017af0b: je       0x17af35
0x0017af0d: push     2
0x0017af0f: push     0
0x0017af11: push     0
0x0017af13: push     1
0x0017af15: push     0
0x0017af17: push     0x683660  ; 'scxlbut.def'
0x0017af1c: push     0x8c
0x0017af21: push     0x21
0x0017af23: push     0x2c
0x0017af25: push     0x34
0x0017af27: push     0x12e
0x0017af2c: mov      ecx, eax
0x0017af2e: call     0x55bd0
0x0017af33: jmp      0x17af37
0x0017af37: lea      ecx, [ebp + 8]
0x0017af3a: mov      byte ptr [ebp - 4], 6
0x0017af3e: mov      dword ptr [ebp + 8], eax
0x0017af41: mov      eax, dword ptr [esi + 8]
0x0017af44: push     ecx
0x0017af45: push     1
0x0017af47: push     eax
0x0017af48: mov      ecx, esi
0x0017af4a: call     0x1fe2d0
0x0017af4f: push     0x68
0x0017af51: call     0x217492
0x0017af56: add      esp, 4
0x0017af59: mov      dword ptr [ebp - 0x10], eax
0x0017af5c: test     eax, eax
0x0017af5e: mov      byte ptr [ebp - 4], 0x27
0x0017af62: je       0x17af8c
0x0017af64: push     2
0x0017af66: push     0
0x0017af68: push     0
0x0017af6a: push     1
0x0017af6c: push     0
0x0017af6e: push     0x683654  ; 'scalbut.def'
0x0017af73: push     0x8d
0x0017af78: push     0x21
0x0017af7a: push     0x2c
0x0017af7c: push     0x34
0x0017af7e: push     0x15d
0x0017af83: mov      ecx, eax
0x0017af85: call     0x55bd0
0x0017af8a: jmp      0x17af8e
0x0017af8e: lea      edx, [ebp + 8]
0x0017af91: mov      byte ptr [ebp - 4], 6
0x0017af95: mov      dword ptr [ebp + 8], eax
0x0017af98: mov      eax, dword ptr [esi + 8]
0x0017af9b: push     edx
0x0017af9c: push     1
0x0017af9e: push     eax
0x0017af9f: mov      ecx, esi
0x0017afa1: call     0x1fe2d0
0x0017afa6: push     0x68
0x0017afa8: call     0x217492
0x0017afad: add      esp, 4
0x0017afb0: mov      dword ptr [ebp - 0x10], eax
0x0017afb3: test     eax, eax
0x0017afb5: mov      byte ptr [ebp - 4], 0x28
0x0017afb9: je       0x17afe0
0x0017afbb: push     2
0x0017afbd: push     0
0x0017afbf: push     0
0x0017afc1: push     1
0x0017afc3: push     0
0x0017afc5: push     0x683648  ; 'scbutt1.def'
0x0017afca: push     0xbe
0x0017afcf: push     0x1c
0x0017afd1: push     0x1f
0x0017afd3: push     0x5c
0x0017afd5: push     0x1a
0x0017afd7: mov      ecx, eax
0x0017afd9: call     0x55bd0
0x0017afde: jmp      0x17afe2
0x0017afe2: lea      ecx, [ebp + 8]
0x0017afe5: mov      byte ptr [ebp - 4], 6
0x0017afe9: mov      dword ptr [ebp + 8], eax
0x0017afec: mov      eax, dword ptr [esi + 8]
0x0017afef: push     ecx
0x0017aff0: push     1
0x0017aff2: push     eax
0x0017aff3: mov      ecx, esi
0x0017aff5: call     0x1fe2d0
0x0017affa: push     0x68
0x0017affc: call     0x217492
0x0017b001: add      esp, 4
0x0017b004: mov      dword ptr [ebp - 0x10], eax
0x0017b007: test     eax, eax
0x0017b009: mov      byte ptr [ebp - 4], 0x29
0x0017b00d: je       0x17b034
0x0017b00f: push     2
0x0017b011: push     0
0x0017b013: push     0
0x0017b015: push     1
0x0017b017: push     0
0x0017b019: push     0x68363c  ; 'scbutt2.def'
0x0017b01e: push     0xbf
0x0017b023: push     0x1c
0x0017b025: push     0x20
0x0017b027: push     0x5c
0x0017b029: push     0x3a
0x0017b02b: mov      ecx, eax
0x0017b02d: call     0x55bd0
0x0017b032: jmp      0x17b036
0x0017b036: lea      edx, [ebp + 8]
0x0017b039: mov      byte ptr [ebp - 4], 6
0x0017b03d: mov      dword ptr [ebp + 8], eax
0x0017b040: mov      eax, dword ptr [esi + 8]
0x0017b043: push     edx
0x0017b044: push     1
0x0017b046: push     eax
0x0017b047: mov      ecx, esi
0x0017b049: call     0x1fe2d0
0x0017b04e: push     0x68
0x0017b050: call     0x217492
0x0017b055: add      esp, 4
0x0017b058: mov      dword ptr [ebp - 0x10], eax
0x0017b05b: test     eax, eax
0x0017b05d: mov      byte ptr [ebp - 4], 0x2a
0x0017b061: je       0x17b088
0x0017b063: push     2
0x0017b065: push     0
0x0017b067: push     0
0x0017b069: push     1
0x0017b06b: push     0
0x0017b06d: push     0x683630  ; 'ScButCp.def'
0x0017b072: push     0xc0
0x0017b077: push     0x1c
0x0017b079: push     0x20
0x0017b07b: push     0x5c
0x0017b07d: push     0x5b
0x0017b07f: mov      ecx, eax
0x0017b081: call     0x55bd0
0x0017b086: jmp      0x17b08a
0x0017b08a: lea      ecx, [ebp + 8]
0x0017b08d: mov      byte ptr [ebp - 4], 6
0x0017b091: mov      dword ptr [ebp + 8], eax
0x0017b094: mov      eax, dword ptr [esi + 8]
0x0017b097: push     ecx
0x0017b098: push     1
0x0017b09a: push     eax
0x0017b09b: mov      ecx, esi
0x0017b09d: call     0x1fe2d0
0x0017b0a2: push     0x68
0x0017b0a4: call     0x217492
0x0017b0a9: add      esp, 4
0x0017b0ac: mov      dword ptr [ebp - 0x10], eax
0x0017b0af: test     eax, eax
0x0017b0b1: mov      byte ptr [ebp - 4], 0x2b
0x0017b0b5: je       0x17b0df
0x0017b0b7: push     2
0x0017b0b9: push     0
0x0017b0bb: push     0
0x0017b0bd: push     1
0x0017b0bf: push     0
0x0017b0c1: push     0x683624  ; 'scbutt3.def'
0x0017b0c6: push     0xc1
0x0017b0cb: push     0x21
0x0017b0cd: push     0xb8
0x0017b0d2: push     0x5c
0x0017b0d4: push     0x7c
0x0017b0d6: mov      ecx, eax
0x0017b0d8: call     0x55bd0
0x0017b0dd: jmp      0x17b0e1
0x0017b0e1: lea      edx, [ebp + 8]
0x0017b0e4: mov      byte ptr [ebp - 4], 6
0x0017b0e8: mov      dword ptr [ebp + 8], eax
0x0017b0eb: mov      eax, dword ptr [esi + 8]
0x0017b0ee: push     edx
0x0017b0ef: push     1
0x0017b0f1: push     eax
0x0017b0f2: mov      ecx, esi
0x0017b0f4: call     0x1fe2d0
0x0017b0f9: push     0x68
0x0017b0fb: call     0x217492
0x0017b100: add      esp, 4
0x0017b103: mov      dword ptr [ebp - 0x10], eax
0x0017b106: test     eax, eax
0x0017b108: mov      byte ptr [ebp - 4], 0x2c
0x0017b10c: je       0x17b136
0x0017b10e: push     2
0x0017b110: push     0
0x0017b112: push     0
0x0017b114: push     1
0x0017b116: push     0
0x0017b118: push     0x683618  ; 'scbutt4.def'
0x0017b11d: push     0xc2
0x0017b122: push     0x1c
0x0017b124: push     0x20
0x0017b126: push     0x5c
0x0017b128: push     0x135
0x0017b12d: mov      ecx, eax
0x0017b12f: call     0x55bd0
0x0017b134: jmp      0x17b138
0x0017b138: lea      ecx, [ebp + 8]
0x0017b13b: mov      byte ptr [ebp - 4], 6
0x0017b13f: mov      dword ptr [ebp + 8], eax
0x0017b142: mov      eax, dword ptr [esi + 8]
0x0017b145: push     ecx
0x0017b146: push     1
0x0017b148: push     eax
0x0017b149: mov      ecx, esi
0x0017b14b: call     0x1fe2d0
0x0017b150: push     0x68
0x0017b152: call     0x217492
0x0017b157: add      esp, 4
0x0017b15a: mov      dword ptr [ebp - 0x10], eax
0x0017b15d: test     eax, eax
0x0017b15f: mov      byte ptr [ebp - 4], 0x2d
0x0017b163: je       0x17b18d
0x0017b165: push     2
0x0017b167: push     0
0x0017b169: push     0
0x0017b16b: push     1
0x0017b16d: push     0
0x0017b16f: push     0x68360c  ; 'scbutt5.def'
0x0017b174: push     0xc3
0x0017b179: push     0x1c
0x0017b17b: push     0x20
0x0017b17d: push     0x5c
0x0017b17f: push     0x156
0x0017b184: mov      ecx, eax
0x0017b186: call     0x55bd0
0x0017b18b: jmp      0x17b18f
0x0017b18f: lea      edx, [ebp + 8]
0x0017b192: mov      byte ptr [ebp - 4], 6
0x0017b196: mov      dword ptr [ebp + 8], eax
0x0017b199: mov      eax, dword ptr [esi + 8]
0x0017b19c: push     edx
0x0017b19d: push     1
0x0017b19f: push     eax
0x0017b1a0: mov      ecx, esi
0x0017b1a2: call     0x1fe2d0
0x0017b1a7: mov      al, byte ptr [ebx + 0x65]
0x0017b1aa: mov      esi, 0x1e0
0x0017b1af: test     al, al
0x0017b1b1: je       0x17b1b8
0x0017b1b3: mov      esi, 0x1ac
0x0017b1b8: push     0x68
0x0017b1ba: call     0x217492
0x0017b1bf: add      esp, 4
0x0017b1c2: mov      dword ptr [ebp + 8], eax
0x0017b1c5: test     eax, eax
0x0017b1c7: mov      byte ptr [ebp - 4], 0x2e
0x0017b1cb: je       0x17b1f7
0x0017b1cd: mov      ecx, dword ptr [0x69fe24]
0x0017b1d3: push     0
0x0017b1d5: push     ecx
0x0017b1d6: push     1
0x0017b1d8: push     0x57cd10
0x0017b1dd: push     0xa
0x0017b1df: push     0x151
0x0017b1e4: push     esi
0x0017b1e5: push     0x10
0x0017b1e7: push     0x5c
0x0017b1e9: push     0x177
0x0017b1ee: mov      ecx, eax
0x0017b1f0: call     0x1963c0
0x0017b1f5: jmp      0x17b1f9
0x0017b1f9: push     6
0x0017b1fb: mov      byte ptr [ebp - 4], 6
0x0017b1ff: mov      ecx, eax
0x0017b201: push     6
0x0017b203: mov      dword ptr [ebx + 0x183c], eax
0x0017b209: call     0x1fed80
0x0017b20e: mov      edx, dword ptr [ebx + 0x183c]
0x0017b214: lea      esi, [ebx + 0x30]
0x0017b217: lea      ecx, [ebp + 8]
0x0017b21a: mov      dword ptr [ebp + 8], edx
0x0017b21d: mov      eax, dword ptr [esi + 8]
0x0017b220: push     ecx
0x0017b221: push     1
0x0017b223: push     eax
0x0017b224: mov      ecx, esi
0x0017b226: call     0x1fe2d0
0x0017b22b: mov      edx, dword ptr [0x683600]  ; 'RBYGOPTS'
0x0017b231: mov      eax, dword ptr [0x683604]
0x0017b236: mov      cl, byte ptr [0x683608]
0x0017b23c: mov      dword ptr [ebp - 0x34], edx
0x0017b23f: mov      dword ptr [ebp - 0x30], eax
0x0017b242: mov      byte ptr [ebp - 0x2c], cl
0x0017b245: mov      al, byte ptr [ebx + 0x64]
0x0017b248: test     al, al
0x0017b24a: jne      0x17b25b
0x0017b24c: mov      cl, byte ptr [ebx + 0x65]
0x0017b24f: test     cl, cl
0x0017b251: je       0x17b271
0x0017b253: test     al, al
0x0017b255: je       0x17baa5
0x0017b25b: mov      eax, dword ptr [0x69959c]
0x0017b260: test     eax, eax
0x0017b262: jne      0x17b271
0x0017b264: cmp      dword ptr [0x698a40], 3
0x0017b26b: jne      0x17be29
0x0017b271: push     0x68
0x0017b273: call     0x217492
0x0017b278: add      esp, 4
0x0017b27b: mov      dword ptr [ebp + 8], eax
0x0017b27e: test     eax, eax
0x0017b280: mov      byte ptr [ebp - 4], 0x2f
0x0017b284: je       0x17b2af
0x0017b286: push     0
0x0017b288: push     0
0x0017b28a: push     1
0x0017b28c: push     0x57cb70
0x0017b291: push     0xb
0x0017b293: push     0x152
0x0017b298: push     0x10
0x0017b29a: push     0xc2
0x0017b29f: push     0x22d
0x0017b2a4: push     0x3a
0x0017b2a6: mov      ecx, eax
0x0017b2a8: call     0x1963c0
0x0017b2ad: jmp      0x17b2b1
0x0017b2b1: mov      esi, dword ptr [ebp - 0x20]
0x0017b2b4: push     6
0x0017b2b6: mov      byte ptr [ebp - 4], 6
0x0017b2ba: mov      ecx, eax
0x0017b2bc: push     6
0x0017b2be: mov      dword ptr [esi + 0x1840], eax
0x0017b2c4: call     0x1fed80
0x0017b2c9: mov      edx, dword ptr [esi + 0x1840]
0x0017b2cf: add      esi, 0x30
0x0017b2d2: lea      ecx, [ebp + 8]
0x0017b2d5: mov      dword ptr [ebp + 8], edx
0x0017b2d8: mov      eax, dword ptr [esi + 8]
0x0017b2db: push     ecx
0x0017b2dc: push     1
0x0017b2de: push     eax
0x0017b2df: mov      ecx, esi
0x0017b2e1: call     0x1fe2d0
0x0017b2e6: mov      edi, 0x107
0x0017b2eb: lea      eax, [ebp - 0x34]
0x0017b2ee: sub      eax, edi
0x0017b2f0: mov      ebx, 0x85
0x0017b2f5: mov      dword ptr [ebp + 8], eax
0x0017b2f8: mov      eax, 0x641af0  ; 'rbygopts'
0x0017b2fd: sub      eax, edi
0x0017b2ff: mov      dword ptr [ebp - 0x14], eax
0x0017b302: mov      edx, dword ptr [ebp + 8]
0x0017b305: lea      ecx, [ebp - 0x1c0]
0x0017b30b: movsx    eax, byte ptr [edx + edi]
0x0017b30f: push     eax
0x0017b310: push     0x6835f0  ; 'AOFLGB%c.DEF'
0x0017b315: push     ecx
0x0017b316: call     0x2179de
0x0017b31b: push     0x68
0x0017b31d: call     0x217492
0x0017b322: add      esp, 0x10
0x0017b325: mov      dword ptr [ebp - 0x10], eax
0x0017b328: test     eax, eax
0x0017b32a: mov      byte ptr [ebp - 4], 0x30
0x0017b32e: je       0x17b364
0x0017b330: mov      ecx, dword ptr [ebp - 0x20]
0x0017b333: push     2
0x0017b335: push     0
0x0017b337: push     0
0x0017b339: push     1
0x0017b33b: lea      edx, [ebp - 0x1c0]
0x0017b341: push     0
0x0017b343: push     edx
0x0017b344: mov      edx, ebx
0x0017b346: push     edi
0x0017b347: sub      edx, dword ptr [ecx + 0x1c]
0x0017b34a: push     0x32
0x0017b34c: push     0x2a
0x0017b34e: sub      edx, 3
0x0017b351: push     edx
0x0017b352: mov      edx, 0xe
0x0017b357: sub      edx, dword ptr [ecx + 0x18]
0x0017b35a: mov      ecx, eax
0x0017b35c: push     edx
0x0017b35d: call     0x55bd0
0x0017b362: jmp      0x17b366
0x0017b366: lea      ecx, [ebp - 0x24]
0x0017b369: mov      byte ptr [ebp - 4], 6
0x0017b36d: mov      dword ptr [ebp - 0x24], eax
0x0017b370: mov      eax, dword ptr [esi + 8]
0x0017b373: push     ecx
0x0017b374: push     1
0x0017b376: push     eax
0x0017b377: mov      ecx, esi
0x0017b379: call     0x1fe2d0
0x0017b37e: push     0x50
0x0017b380: call     0x217492
0x0017b385: add      esp, 4
0x0017b388: mov      dword ptr [ebp - 0x10], eax
0x0017b38b: test     eax, eax
0x0017b38d: mov      byte ptr [ebp - 4], 0x31
0x0017b391: je       0x17b3c7
0x0017b393: mov      edx, dword ptr [0x6a5dc4]
0x0017b399: push     8
0x0017b39b: push     0
0x0017b39d: push     5
0x0017b39f: mov      ecx, dword ptr [edx + 0x20]
0x0017b3a2: lea      edx, [edi - 0x40]
0x0017b3a5: push     edx
0x0017b3a6: push     4
0x0017b3a8: mov      ecx, dword ptr [ecx + 0x7d0]
0x0017b3ae: push     0x660cb4  ; 'tiny.fnt'
0x0017b3b3: push     ecx
0x0017b3b4: push     0x18
0x0017b3b6: lea      ecx, [ebx + 0x12]
0x0017b3b9: push     0x2e
0x0017b3bb: push     ecx
0x0017b3bc: push     0x3e
0x0017b3be: mov      ecx, eax
0x0017b3c0: call     0x1bc6a0
0x0017b3c5: jmp      0x17b3c9
0x0017b3c9: lea      edx, [ebp - 0x18]
0x0017b3cc: mov      byte ptr [ebp - 4], 6
0x0017b3d0: mov      dword ptr [ebp - 0x18], eax
0x0017b3d3: mov      eax, dword ptr [esi + 8]
0x0017b3d6: push     edx
0x0017b3d7: push     1
0x0017b3d9: push     eax
0x0017b3da: mov      ecx, esi
0x0017b3dc: call     0x1fe2d0
0x0017b3e1: mov      eax, dword ptr [ebp - 0x14]
0x0017b3e4: lea      edx, [ebp - 0xc0]
0x0017b3ea: movsx    ecx, byte ptr [eax + edi]
0x0017b3ee: push     ecx
0x0017b3ef: push     0x6835e0  ; 'adopb2%c.def'
0x0017b3f4: push     edx
0x0017b3f5: call     0x2179de
0x0017b3fa: mov      eax, dword ptr [0x69959c]
0x0017b3ff: add      esp, 0xc
0x0017b402: test     eax, eax
0x0017b404: jne      0x17b40f
0x0017b406: cmp      dword ptr [0x698a40], 3
0x0017b40d: jne      0x17b45a
0x0017b40f: push     0x70
0x0017b411: call     0x217492
0x0017b416: add      esp, 4
0x0017b419: mov      dword ptr [ebp - 0x10], eax
0x0017b41c: test     eax, eax
0x0017b41e: mov      byte ptr [ebp - 4], 0x32
0x0017b422: je       0x17b497
0x0017b424: mov      ecx, dword ptr [0x6a7868]
0x0017b42a: push     4
0x0017b42c: push     2
0x0017b42e: push     0
0x0017b430: push     0
0x0017b432: push     1
0x0017b434: push     0
0x0017b436: push     0x660cb4  ; 'tiny.fnt'
0x0017b43b: push     ecx
0x0017b43c: lea      edx, [ebp - 0xc0]
0x0017b442: lea      ecx, [edi - 0x38]
0x0017b445: push     edx
0x0017b446: push     ecx
0x0017b447: push     0x18
0x0017b449: lea      edx, [ebx + 0x12]
0x0017b44c: push     0x32
0x0017b44e: push     edx
0x0017b44f: push     0x6e
0x0017b451: mov      ecx, eax
0x0017b453: call     0x56730
0x0017b458: jmp      0x17b499
0x0017b499: lea      edx, [ebp - 0x28]
0x0017b49c: mov      byte ptr [ebp - 4], 6
0x0017b4a0: mov      dword ptr [ebp - 0x28], eax
0x0017b4a3: mov      eax, dword ptr [esi + 8]
0x0017b4a6: push     edx
0x0017b4a7: push     1
0x0017b4a9: push     eax
0x0017b4aa: mov      ecx, esi
0x0017b4ac: call     0x1fe2d0
0x0017b4b1: push     0x68
0x0017b4b3: call     0x217492
0x0017b4b8: add      esp, 4
0x0017b4bb: mov      dword ptr [ebp - 0x10], eax
0x0017b4be: test     eax, eax
0x0017b4c0: mov      byte ptr [ebp - 4], 0x34
0x0017b4c4: je       0x17b4ec
0x0017b4c6: push     2
0x0017b4c8: push     0
0x0017b4ca: push     0
0x0017b4cc: push     1
0x0017b4ce: push     0
0x0017b4d0: lea      ecx, [edi - 0x30]
0x0017b4d3: push     0x6835d4  ; 'adoplfa.def'
0x0017b4d8: push     ecx
0x0017b4d9: push     0x18
0x0017b4db: push     0xb
0x0017b4dd: push     ebx
0x0017b4de: push     0xa4
0x0017b4e3: mov      ecx, eax
0x0017b4e5: call     0x55bd0
0x0017b4ea: jmp      0x17b4ee
0x0017b4ee: lea      edx, [ebp - 0x44]
0x0017b4f1: mov      byte ptr [ebp - 4], 6
0x0017b4f5: mov      dword ptr [ebp - 0x44], eax
0x0017b4f8: mov      eax, dword ptr [esi + 8]
0x0017b4fb: push     edx
0x0017b4fc: push     1
0x0017b4fe: push     eax
0x0017b4ff: mov      ecx, esi
0x0017b501: call     0x1fe2d0
0x0017b506: push     0x68
0x0017b508: call     0x217492
0x0017b50d: add      esp, 4
0x0017b510: mov      dword ptr [ebp - 0x10], eax
0x0017b513: test     eax, eax
0x0017b515: mov      byte ptr [ebp - 4], 0x35
0x0017b519: je       0x17b541
0x0017b51b: push     2
0x0017b51d: push     0
0x0017b51f: push     0
0x0017b521: push     0
0x0017b523: push     1
0x0017b525: lea      ecx, [edi - 0x28]
0x0017b528: push     0x6835c8  ; 'adoprta.def'
0x0017b52d: push     ecx
0x0017b52e: push     0x18
0x0017b530: push     0xb
0x0017b532: push     ebx
0x0017b533: push     0xe1
0x0017b538: mov      ecx, eax
0x0017b53a: call     0x55bd0
0x0017b53f: jmp      0x17b543
0x0017b543: lea      edx, [ebp - 0x40]
0x0017b546: mov      byte ptr [ebp - 4], 6
0x0017b54a: mov      dword ptr [ebp - 0x40], eax
0x0017b54d: mov      eax, dword ptr [esi + 8]
0x0017b550: push     edx
0x0017b551: push     1
0x0017b553: push     eax
0x0017b554: mov      ecx, esi
0x0017b556: call     0x1fe2d0
0x0017b55b: push     0x68
0x0017b55d: call     0x217492
0x0017b562: add      esp, 4
0x0017b565: mov      dword ptr [ebp - 0x10], eax
0x0017b568: test     eax, eax
0x0017b56a: mov      byte ptr [ebp - 4], 0x36
0x0017b56e: je       0x17b596
0x0017b570: push     2
0x0017b572: push     0
0x0017b574: push     0
0x0017b576: push     1
0x0017b578: push     0
0x0017b57a: lea      ecx, [edi - 0x20]
0x0017b57d: push     0x6835d4  ; 'adoplfa.def'
0x0017b582: push     ecx
0x0017b583: push     0x18
0x0017b585: push     0xb
0x0017b587: push     ebx
0x0017b588: push     0xf0
0x0017b58d: mov      ecx, eax
0x0017b58f: call     0x55bd0
0x0017b594: jmp      0x17b598
0x0017b598: lea      edx, [ebp - 0x48]
0x0017b59b: mov      byte ptr [ebp - 4], 6
0x0017b59f: mov      dword ptr [ebp - 0x48], eax
0x0017b5a2: mov      eax, dword ptr [esi + 8]
0x0017b5a5: push     edx
0x0017b5a6: push     1
0x0017b5a8: push     eax
0x0017b5a9: mov      ecx, esi
0x0017b5ab: call     0x1fe2d0
0x0017b5b0: push     0x68
0x0017b5b2: call     0x217492
0x0017b5b7: add      esp, 4
0x0017b5ba: mov      dword ptr [ebp - 0x10], eax
0x0017b5bd: test     eax, eax
0x0017b5bf: mov      byte ptr [ebp - 4], 0x37
0x0017b5c3: je       0x17b5eb
0x0017b5c5: push     2
0x0017b5c7: push     0
0x0017b5c9: push     0
0x0017b5cb: push     0
0x0017b5cd: push     1
0x0017b5cf: lea      ecx, [edi - 0x18]
0x0017b5d2: push     0x6835c8  ; 'adoprta.def'
0x0017b5d7: push     ecx
0x0017b5d8: push     0x18
0x0017b5da: push     0xb
0x0017b5dc: push     ebx
0x0017b5dd: push     0x12d
0x0017b5e2: mov      ecx, eax
0x0017b5e4: call     0x55bd0
0x0017b5e9: jmp      0x17b5ed
0x0017b5ed: lea      edx, [ebp - 0x50]
0x0017b5f0: mov      byte ptr [ebp - 4], 6
0x0017b5f4: mov      dword ptr [ebp - 0x50], eax
0x0017b5f7: mov      eax, dword ptr [esi + 8]
0x0017b5fa: push     edx
0x0017b5fb: push     1
0x0017b5fd: push     eax
0x0017b5fe: mov      ecx, esi
0x0017b600: call     0x1fe2d0
0x0017b605: push     0x68
0x0017b607: call     0x217492
0x0017b60c: add      esp, 4
0x0017b60f: mov      dword ptr [ebp - 0x10], eax
0x0017b612: test     eax, eax
0x0017b614: mov      byte ptr [ebp - 4], 0x38
0x0017b618: je       0x17b640
0x0017b61a: push     2
0x0017b61c: push     0
0x0017b61e: push     0
0x0017b620: push     1
0x0017b622: push     0
0x0017b624: lea      ecx, [edi - 0x10]
0x0017b627: push     0x6835d4  ; 'adoplfa.def'
0x0017b62c: push     ecx
0x0017b62d: push     0x18
0x0017b62f: push     0xb
0x0017b631: push     ebx
0x0017b632: push     0x13c
0x0017b637: mov      ecx, eax
0x0017b639: call     0x55bd0
0x0017b63e: jmp      0x17b642
0x0017b642: lea      edx, [ebp - 0x3c]
0x0017b645: mov      byte ptr [ebp - 4], 6
0x0017b649: mov      dword ptr [ebp - 0x3c], eax
0x0017b64c: mov      eax, dword ptr [esi + 8]
0x0017b64f: push     edx
0x0017b650: push     1
0x0017b652: push     eax
0x0017b653: mov      ecx, esi
0x0017b655: call     0x1fe2d0
0x0017b65a: push     0x68
0x0017b65c: call     0x217492
0x0017b661: add      esp, 4
0x0017b664: mov      dword ptr [ebp - 0x10], eax
0x0017b667: test     eax, eax
0x0017b669: mov      byte ptr [ebp - 4], 0x39
0x0017b66d: je       0x17b695
0x0017b66f: push     2
0x0017b671: push     0
0x0017b673: push     0
0x0017b675: push     0
0x0017b677: push     1
0x0017b679: lea      ecx, [edi - 8]
0x0017b67c: push     0x6835c8  ; 'adoprta.def'
0x0017b681: push     ecx
0x0017b682: push     0x18
0x0017b684: push     0xb
0x0017b686: push     ebx
0x0017b687: push     0x179
0x0017b68c: mov      ecx, eax
0x0017b68e: call     0x55bd0
0x0017b693: jmp      0x17b697
0x0017b697: lea      edx, [ebp - 0x5c]
0x0017b69a: mov      byte ptr [ebp - 4], 6
0x0017b69e: mov      dword ptr [ebp - 0x5c], eax
0x0017b6a1: mov      eax, dword ptr [esi + 8]
0x0017b6a4: push     edx
0x0017b6a5: push     1
0x0017b6a7: push     eax
0x0017b6a8: mov      ecx, esi
0x0017b6aa: call     0x1fe2d0
0x0017b6af: push     0x50
0x0017b6b1: call     0x217492
0x0017b6b6: add      esp, 4
0x0017b6b9: mov      dword ptr [ebp - 0x10], eax
0x0017b6bc: test     eax, eax
0x0017b6be: mov      byte ptr [ebp - 4], 0x3a
0x0017b6c2: je       0x17b6f8
0x0017b6c4: mov      ecx, dword ptr [0x6a5dc4]
0x0017b6ca: push     8
0x0017b6cc: push     0
0x0017b6ce: lea      edx, [edi + 0x52]
0x0017b6d1: mov      ecx, dword ptr [ecx + 0x20]
0x0017b6d4: push     1
0x0017b6d6: push     edx
0x0017b6d7: push     1
0x0017b6d9: mov      ecx, dword ptr [ecx + 0x754]
0x0017b6df: push     0x65f2f8  ; 'smalfont.fnt'
0x0017b6e4: push     ecx
0x0017b6e5: push     0x11
0x0017b6e7: lea      ecx, [ebx - 3]
0x0017b6ea: push     0x61
0x0017b6ec: push     ecx
0x0017b6ed: push     0x3e
0x0017b6ef: mov      ecx, eax
0x0017b6f1: call     0x1bc6a0
0x0017b6f6: jmp      0x17b6fa
0x0017b6fa: lea      edx, [ebp - 0x4c]
0x0017b6fd: mov      byte ptr [ebp - 4], 6
0x0017b701: mov      dword ptr [ebp - 0x4c], eax
0x0017b704: mov      eax, dword ptr [esi + 8]
0x0017b707: push     edx
0x0017b708: push     1
0x0017b70a: push     eax
0x0017b70b: mov      ecx, esi
0x0017b70d: call     0x1fe2d0
0x0017b712: push     0x30
0x0017b714: call     0x217492
0x0017b719: add      esp, 4
0x0017b71c: mov      dword ptr [ebp - 0x10], eax
0x0017b71f: test     eax, eax
0x0017b721: mov      byte ptr [ebp - 4], 0x3b
0x0017b725: je       0x17b741
0x0017b727: lea      ecx, [edi + 0x63]
0x0017b72a: lea      edx, [ebx - 3]
0x0017b72d: push     ecx
0x0017b72e: push     0x20
0x0017b730: push     0x30
0x0017b732: push     edx
0x0017b733: push     0xfc
0x0017b738: mov      ecx, eax
0x0017b73a: call     0x1755c0
0x0017b73f: jmp      0x17b743
0x0017b743: lea      ecx, [ebp - 0x58]
0x0017b746: mov      byte ptr [ebp - 4], 6
0x0017b74a: mov      dword ptr [ebp - 0x58], eax
0x0017b74d: mov      eax, dword ptr [esi + 8]
0x0017b750: push     ecx
0x0017b751: push     1
0x0017b753: push     eax
0x0017b754: mov      ecx, esi
0x0017b756: call     0x1fe2d0
0x0017b75b: push     0x30
0x0017b75d: call     0x217492
0x0017b762: add      esp, 4
0x0017b765: mov      dword ptr [ebp - 0x10], eax
0x0017b768: test     eax, eax
0x0017b76a: mov      byte ptr [ebp - 4], 0x3c
0x0017b76e: je       0x17b78a
0x0017b770: lea      edx, [edi + 0x6b]
0x0017b773: lea      ecx, [ebx - 3]
0x0017b776: push     edx
0x0017b777: push     0x20
0x0017b779: push     0x30
0x0017b77b: push     ecx
0x0017b77c: push     0xb0
0x0017b781: mov      ecx, eax
0x0017b783: call     0x1755c0
0x0017b788: jmp      0x17b78c
0x0017b78c: lea      edx, [ebp - 0x54]
0x0017b78f: mov      byte ptr [ebp - 4], 6
0x0017b793: mov      dword ptr [ebp - 0x54], eax
0x0017b796: mov      eax, dword ptr [esi + 8]
0x0017b799: push     edx
0x0017b79a: push     1
0x0017b79c: push     eax
0x0017b79d: mov      ecx, esi
0x0017b79f: call     0x1fe2d0
0x0017b7a4: push     0x30
0x0017b7a6: call     0x217492
0x0017b7ab: add      esp, 4
0x0017b7ae: mov      dword ptr [ebp - 0x10], eax
0x0017b7b1: test     eax, eax
0x0017b7b3: mov      byte ptr [ebp - 4], 0x3d
0x0017b7b7: je       0x17b7d3
0x0017b7b9: lea      ecx, [edi + 0x73]
0x0017b7bc: lea      edx, [ebx - 3]
0x0017b7bf: push     ecx
0x0017b7c0: push     0x20
0x0017b7c2: push     0x30
0x0017b7c4: push     edx
0x0017b7c5: push     0x148
0x0017b7ca: mov      ecx, eax
0x0017b7cc: call     0x1755c0
0x0017b7d1: jmp      0x17b7d5
0x0017b7d5: lea      ecx, [ebp - 0x1c]
0x0017b7d8: mov      byte ptr [ebp - 4], 6
0x0017b7dc: mov      dword ptr [ebp - 0x1c], eax
0x0017b7df: mov      eax, dword ptr [esi + 8]
0x0017b7e2: push     ecx
0x0017b7e3: push     1
0x0017b7e5: push     eax
0x0017b7e6: mov      ecx, esi
0x0017b7e8: call     0x1fe2d0
0x0017b7ed: mov      eax, dword ptr [0x69959c]
0x0017b7f2: test     eax, eax
0x0017b7f4: jne      0x17b88e
0x0017b7fa: cmp      dword ptr [0x698a40], 3
0x0017b801: je       0x17b88e
0x0017b807: push     0x70
0x0017b809: call     0x217492
0x0017b80e: mov      esi, eax
0x0017b810: add      esp, 4
0x0017b813: mov      dword ptr [ebp - 0x10], esi
0x0017b816: test     esi, esi
0x0017b818: mov      byte ptr [ebp - 4], 0x3e
0x0017b81c: je       0x17b85a
0x0017b81e: push     5
0x0017b820: push     7
0x0017b822: push     0
0x0017b824: lea      edx, [edi + 0x5a]
0x0017b827: push     0x100
0x0017b82c: push     edx
0x0017b82d: push     0
0x0017b82f: push     0
0x0017b831: push     0
0x0017b833: push     1
0x0017b835: push     0x65f2f8  ; 'smalfont.fnt'
0x0017b83a: push     0x691260
0x0017b83f: push     0x15
0x0017b841: push     0x11
0x0017b843: lea      eax, [ebx - 3]
0x0017b846: push     0x61
0x0017b848: push     eax
0x0017b849: push     0x3e
0x0017b84b: mov      ecx, esi
0x0017b84d: call     0x1bacd0
0x0017b852: mov      dword ptr [esi], 0x641c24
0x0017b858: jmp      0x17b85c
0x0017b85c: push     6
0x0017b85e: push     6
0x0017b860: mov      ecx, esi
0x0017b862: mov      byte ptr [ebp - 4], 6
0x0017b866: call     0x1fed80
0x0017b86b: mov      edx, dword ptr [esi]
0x0017b86d: push     1
0x0017b86f: mov      ecx, esi
0x0017b871: call     dword ptr [edx + 0x44]
0x0017b874: mov      eax, dword ptr [ebp - 0x20]
0x0017b877: mov      dword ptr [ebp - 0x10], esi
0x0017b87a: lea      ecx, [ebp - 0x10]
0x0017b87d: lea      esi, [eax + 0x30]
0x0017b880: push     ecx
0x0017b881: push     1
0x0017b883: mov      ecx, esi
0x0017b885: mov      eax, dword ptr [esi + 8]
0x0017b888: push     eax
0x0017b889: call     0x1fe2d0
0x0017b88e: inc      edi
0x0017b88f: add      ebx, 0x32
0x0017b892: lea      edx, [edi - 0x107]
0x0017b898: cmp      edx, 8
0x0017b89b: jl       0x17b302
0x0017b8a1: push     0x50
0x0017b8a3: call     0x217492
0x0017b8a8: add      esp, 4
0x0017b8ab: mov      dword ptr [ebp - 0x10], eax
0x0017b8ae: test     eax, eax
0x0017b8b0: mov      byte ptr [ebp - 4], 0x3f
0x0017b8b4: je       0x17b8e9
0x0017b8b6: mov      ecx, dword ptr [0x6a5dc4]
0x0017b8bc: push     8
0x0017b8be: push     0
0x0017b8c0: push     5
0x0017b8c2: mov      edx, dword ptr [ecx + 0x20]
0x0017b8c5: push     0x153
0x0017b8ca: push     2
0x0017b8cc: push     0x65f2f8  ; 'smalfont.fnt'
0x0017b8d1: mov      edx, dword ptr [edx + 0x818]
0x0017b8d7: mov      ecx, eax
0x0017b8d9: push     edx
0x0017b8da: push     0x24
0x0017b8dc: push     0x68
0x0017b8de: push     0x5a
0x0017b8e0: push     0x3a
0x0017b8e2: call     0x1bc6a0
0x0017b8e7: jmp      0x17b8eb
0x0017b8eb: mov      edi, dword ptr [ebp - 0x20]
0x0017b8ee: lea      edx, [ebp + 8]
0x0017b8f1: mov      byte ptr [ebp - 4], 6
0x0017b8f5: mov      dword ptr [ebp + 8], eax
0x0017b8f8: mov      eax, dword ptr [edi + 0x38]
0x0017b8fb: lea      esi, [edi + 0x30]
0x0017b8fe: push     edx
0x0017b8ff: push     1
0x0017b901: push     eax
0x0017b902: mov      ecx, esi
0x0017b904: call     0x1fe2d0
0x0017b909: push     0x50
0x0017b90b: call     0x217492
0x0017b910: add      esp, 4
0x0017b913: mov      dword ptr [ebp - 0x10], eax
0x0017b916: test     eax, eax
0x0017b918: mov      byte ptr [ebp - 4], 0x40
0x0017b91c: je       0x17b956
0x0017b91e: mov      ecx, dword ptr [0x6a5dc4]
0x0017b924: mov      edx, dword ptr [edi + 0x189c]
0x0017b92a: push     8
0x0017b92c: push     0
0x0017b92e: mov      ecx, dword ptr [ecx + 0x20]
0x0017b931: push     5
0x0017b933: push     edx
0x0017b934: push     2
0x0017b936: mov      ecx, dword ptr [ecx + 0x81c]
0x0017b93c: push     0x65f2f8  ; 'smalfont.fnt'
0x0017b941: push     ecx
0x0017b942: push     0x24
0x0017b944: push     0x4b
0x0017b946: push     0x5a
0x0017b948: push     0xa3
0x0017b94d: mov      ecx, eax
0x0017b94f: call     0x1bc6a0
0x0017b954: jmp      0x17b958
0x0017b958: lea      ecx, [ebp + 8]
0x0017b95b: mov      byte ptr [ebp - 4], 6
0x0017b95f: mov      dword ptr [ebp + 8], eax
0x0017b962: mov      eax, dword ptr [esi + 8]
0x0017b965: push     ecx
0x0017b966: push     1
0x0017b968: push     eax
0x0017b969: mov      ecx, esi
0x0017b96b: call     0x1fe2d0
0x0017b970: push     0x50
0x0017b972: call     0x217492
0x0017b977: add      esp, 4
0x0017b97a: mov      dword ptr [ebp - 0x10], eax
0x0017b97d: test     eax, eax
0x0017b97f: mov      byte ptr [ebp - 4], 0x41
0x0017b983: je       0x17b9bb
0x0017b985: mov      edx, dword ptr [0x6a5dc4]
0x0017b98b: push     8
0x0017b98d: push     0
0x0017b98f: push     5
0x0017b991: mov      ecx, dword ptr [edx + 0x20]
0x0017b994: push     0x157
0x0017b999: push     2
0x0017b99b: push     0x65f2f8  ; 'smalfont.fnt'
0x0017b9a0: mov      ecx, dword ptr [ecx + 0x820]
0x0017b9a6: push     ecx
0x0017b9a7: push     0x24
0x0017b9a9: push     0x4b
0x0017b9ab: push     0x5a
0x0017b9ad: push     0xef
0x0017b9b2: mov      ecx, eax
0x0017b9b4: call     0x1bc6a0
0x0017b9b9: jmp      0x17b9bd
0x0017b9bd: lea      ecx, [ebp + 8]
0x0017b9c0: mov      byte ptr [ebp - 4], 6
0x0017b9c4: mov      dword ptr [ebp + 8], eax
0x0017b9c7: mov      eax, dword ptr [esi + 8]
0x0017b9ca: push     ecx
0x0017b9cb: push     1
0x0017b9cd: push     eax
0x0017b9ce: mov      ecx, esi
0x0017b9d0: call     0x1fe2d0
0x0017b9d5: push     0x50
0x0017b9d7: call     0x217492
0x0017b9dc: add      esp, 4
0x0017b9df: mov      dword ptr [ebp - 0x10], eax
0x0017b9e2: test     eax, eax
0x0017b9e4: mov      byte ptr [ebp - 4], 0x42
0x0017b9e8: je       0x17ba20
0x0017b9ea: mov      edx, dword ptr [0x6a5dc4]
0x0017b9f0: push     8
0x0017b9f2: push     0
0x0017b9f4: push     5
0x0017b9f6: mov      ecx, dword ptr [edx + 0x20]
0x0017b9f9: push     0x158
0x0017b9fe: push     2
0x0017ba00: push     0x65f2f8  ; 'smalfont.fnt'
0x0017ba05: mov      ecx, dword ptr [ecx + 0x824]
0x0017ba0b: push     ecx
0x0017ba0c: push     0x24
0x0017ba0e: push     0x4b
0x0017ba10: push     0x5a
0x0017ba12: push     0x13b
0x0017ba17: mov      ecx, eax
0x0017ba19: call     0x1bc6a0
0x0017ba1e: jmp      0x17ba22
0x0017ba22: lea      ecx, [ebp + 8]
0x0017ba25: mov      byte ptr [ebp - 4], 6
0x0017ba29: mov      dword ptr [ebp + 8], eax
0x0017ba2c: mov      eax, dword ptr [esi + 8]
0x0017ba2f: push     ecx
0x0017ba30: push     1
0x0017ba32: push     eax
0x0017ba33: mov      ecx, esi
0x0017ba35: call     0x1fe2d0
0x0017ba3a: push     0x50
0x0017ba3c: call     0x217492
0x0017ba41: add      esp, 4
0x0017ba44: mov      dword ptr [ebp - 0x10], eax
0x0017ba47: test     eax, eax
0x0017ba49: mov      byte ptr [ebp - 4], 0x43
0x0017ba4d: je       0x17ba88
0x0017ba4f: mov      edx, dword ptr [0x6a5dc4]
0x0017ba55: push     8
0x0017ba57: push     0
0x0017ba59: push     5
0x0017ba5b: mov      ecx, dword ptr [edx + 0x20]
0x0017ba5e: push     0x154
0x0017ba63: push     2
0x0017ba65: push     0x65f2f8  ; 'smalfont.fnt'
0x0017ba6a: mov      ecx, dword ptr [ecx + 0x828]
0x0017ba70: push     ecx
0x0017ba71: push     0x14
0x0017ba73: push     0x14e
0x0017ba78: push     0x216
0x0017ba7d: push     0x3a
0x0017ba7f: mov      ecx, eax
0x0017ba81: call     0x1bc6a0
0x0017ba86: jmp      0x17ba8a
0x0017ba8a: lea      ecx, [ebp + 8]
0x0017ba8d: mov      byte ptr [ebp - 4], 6
0x0017ba91: mov      dword ptr [ebp + 8], eax
0x0017ba94: mov      eax, dword ptr [esi + 8]
0x0017ba97: push     ecx
0x0017ba98: push     1
0x0017ba9a: push     eax
0x0017ba9b: mov      ecx, esi
0x0017ba9d: call     0x1fe2d0
0x0017baa2: mov      ebx, dword ptr [ebp - 0x20]
0x0017baa5: mov      eax, dword ptr [0x69959c]
0x0017baaa: test     eax, eax
0x0017baac: je       0x17be29
0x0017bab2: mov      al, byte ptr [ebx + 0x65]
0x0017bab5: test     al, al
0x0017bab7: jne      0x17be29
0x0017babd: push     0x54
0x0017babf: call     0x217492
0x0017bac4: mov      edi, eax
0x0017bac6: add      esp, 4
0x0017bac9: mov      dword ptr [ebp + 8], edi
0x0017bacc: test     edi, edi
0x0017bace: mov      byte ptr [ebp - 4], 0x44
0x0017bad2: je       0x17bb4b
0x0017bad4: push     8
0x0017bad6: push     0
0x0017bad8: push     8
0x0017bada: push     0xb3
0x0017badf: push     0xd
0x0017bae1: push     0x65f2f8  ; 'smalfont.fnt'
0x0017bae6: push     0
0x0017bae8: push     0x80
0x0017baed: push     0x13b
0x0017baf2: push     0x83
0x0017baf7: push     0x1a0
0x0017bafc: mov      ecx, edi
0x0017bafe: call     0x1bc6a0
0x0017bb03: mov      byte ptr [ebp - 4], 0x45
0x0017bb07: push     0x3c
0x0017bb09: mov      dword ptr [edi], 0x641bec
0x0017bb0f: call     0x217492
0x0017bb14: mov      esi, eax
0x0017bb16: add      esp, 4
0x0017bb19: mov      dword ptr [ebp - 0x10], esi
0x0017bb1c: test     esi, esi
0x0017bb1e: mov      byte ptr [ebp - 4], 0x46
0x0017bb22: je       0x17bb44
0x0017bb24: push     0x80
0x0017bb29: push     0x13b
0x0017bb2e: mov      ecx, esi
0x0017bb30: call     0x4dc40
0x0017bb35: mov      dword ptr [esi], 0x641be0
0x0017bb3b: mov      byte ptr [esi + 0x38], 0
0x0017bb3f: mov      dword ptr [edi + 0x50], esi
0x0017bb42: jmp      0x17bb4d
0x0017bb4d: mov      byte ptr [ebp - 4], 6
0x0017bb51: push     0x68
0x0017bb53: mov      dword ptr [ebx + 0x1848], edi
0x0017bb59: call     0x217492
0x0017bb5e: mov      esi, eax
0x0017bb60: add      esp, 4
0x0017bb63: mov      dword ptr [ebp + 8], esi
0x0017bb66: test     esi, esi
0x0017bb68: mov      byte ptr [ebp - 4], 0x47
0x0017bb6c: je       0x17bb9d
0x0017bb6e: push     0
0x0017bb70: push     0
0x0017bb72: push     1
0x0017bb74: push     0x57cb50
0x0017bb79: push     0
0x0017bb7b: push     0x150
0x0017bb80: push     0x7e
0x0017bb82: push     0x10
0x0017bb84: push     0x85
0x0017bb89: push     0x2dd
0x0017bb8e: mov      ecx, esi
0x0017bb90: call     0x1963c0
0x0017bb95: mov      dword ptr [esi], 0x641b9c
0x0017bb9b: jmp      0x17bb9f
0x0017bb9f: mov      ecx, esi
0x0017bba1: mov      byte ptr [ebp - 4], 6
0x0017bba5: mov      dword ptr [ebx + 0x1838], esi
0x0017bbab: push     0
0x0017bbad: mov      edx, dword ptr [ecx]
0x0017bbaf: call     dword ptr [edx + 0x34]
0x0017bbb2: mov      ecx, dword ptr [ebx + 0x1838]
0x0017bbb8: push     0
0x0017bbba: mov      eax, dword ptr [ecx]
0x0017bbbc: call     dword ptr [eax + 0x38]
0x0017bbbf: mov      ecx, dword ptr [ebx + 0x1848]
0x0017bbc5: push     5
0x0017bbc7: push     0
0x0017bbc9: push     ecx
0x0017bbca: mov      ecx, 0x69d800
0x0017bbcf: call     0x154100
0x0017bbd4: push     -1
0x0017bbd6: mov      ecx, 0x69d800
0x0017bbdb: call     0x154570
0x0017bbe0: push     0x70
0x0017bbe2: call     0x217492
0x0017bbe7: mov      esi, eax
0x0017bbe9: add      esp, 4
0x0017bbec: mov      dword ptr [ebp + 8], esi
0x0017bbef: test     esi, esi
0x0017bbf1: mov      byte ptr [ebp - 4], 0x48
0x0017bbf5: je       0x17bc3e
0x0017bbf7: push     5
0x0017bbf9: push     7
0x0017bbfb: push     0
0x0017bbfd: push     0x100
0x0017bc02: push     0xb4
0x0017bc07: push     0
0x0017bc09: push     0x6835b8  ; 'ircentry.pcx'
0x0017bc0e: push     0
0x0017bc10: push     4
0x0017bc12: push     0x65f2f8  ; 'smalfont.fnt'
0x0017bc17: push     0x691260
0x0017bc1c: push     0x7f
0x0017bc1e: push     0x10
0x0017bc20: push     0x14e
0x0017bc25: push     0x103
0x0017bc2a: push     0x19f
0x0017bc2f: mov      ecx, esi
0x0017bc31: call     0x1545a0
0x0017bc36: mov      dword ptr [esi], 0x641b38
0x0017bc3c: jmp      0x17bc40
0x0017bc40: mov      byte ptr [ebp - 4], 6
0x0017bc44: push     0x34
0x0017bc46: mov      dword ptr [ebx + 0x1858], esi
0x0017bc4c: call     0x217492
0x0017bc51: add      esp, 4
0x0017bc54: mov      dword ptr [ebp + 8], eax
0x0017bc57: test     eax, eax
0x0017bc59: mov      byte ptr [ebp - 4], 0x49
0x0017bc5d: je       0x17bc8a
0x0017bc5f: push     0x800
0x0017bc64: push     0x6835a8  ; 'CHATPLUG.pcx'
0x0017bc69: push     0xb5
0x0017bc6e: push     0x72
0x0017bc70: push     0x154
0x0017bc75: push     0x11a
0x0017bc7a: push     0x19c
0x0017bc7f: mov      ecx, eax
0x0017bc81: call     0x4ffa0
0x0017bc86: mov      esi, eax
0x0017bc88: jmp      0x17bc8c
0x0017bc8c: push     0x50
0x0017bc8e: mov      byte ptr [ebp - 4], 6
0x0017bc92: call     0x217492
0x0017bc97: add      esp, 4
0x0017bc9a: mov      dword ptr [ebp + 8], eax
0x0017bc9d: test     eax, eax
0x0017bc9f: mov      byte ptr [ebp - 4], 0x4a
0x0017bca3: je       0x17bcd3
0x0017bca5: push     8
0x0017bca7: push     0
0x0017bca9: push     0
0x0017bcab: push     0xb6
0x0017bcb0: push     1
0x0017bcb2: push     0x65f2f8  ; 'smalfont.fnt'
0x0017bcb7: push     0
0x0017bcb9: push     0x67
0x0017bcbb: push     0x9c
0x0017bcc0: push     0x11f
0x0017bcc5: push     0x1a3
0x0017bcca: mov      ecx, eax
0x0017bccc: call     0x1bc6a0
0x0017bcd1: jmp      0x17bcd5
0x0017bcd5: mov      byte ptr [ebp - 4], 6
0x0017bcd9: push     0x50
0x0017bcdb: mov      dword ptr [ebx + 0x184c], eax
0x0017bce1: call     0x217492
0x0017bce6: add      esp, 4
0x0017bce9: mov      dword ptr [ebp + 8], eax
0x0017bcec: test     eax, eax
0x0017bcee: mov      byte ptr [ebp - 4], 0x4b
0x0017bcf2: je       0x17bd22
0x0017bcf4: push     8
0x0017bcf6: push     0
0x0017bcf8: push     0
0x0017bcfa: push     0xb6
0x0017bcff: push     1
0x0017bd01: push     0x65f2f8  ; 'smalfont.fnt'
0x0017bd06: push     0
0x0017bd08: push     0x67
0x0017bd0a: push     0x9c
0x0017bd0f: push     0x11f
0x0017bd14: push     0x248
0x0017bd19: mov      ecx, eax
0x0017bd1b: call     0x1bc6a0
0x0017bd20: jmp      0x17bd24
0x0017bd24: mov      byte ptr [ebp - 4], 6
0x0017bd28: mov      dword ptr [ebx + 0x1850], eax
0x0017bd2e: mov      dword ptr [ebp + 8], esi
0x0017bd31: mov      eax, dword ptr [ebx + 0x38]
0x0017bd34: lea      esi, [ebx + 0x30]
0x0017bd37: lea      edx, [ebp + 8]
0x0017bd3a: push     edx
0x0017bd3b: push     1
0x0017bd3d: push     eax
0x0017bd3e: mov      ecx, esi
0x0017bd40: call     0x1fe2d0
0x0017bd45: push     0x34
0x0017bd47: call     0x217492
0x0017bd4c: add      esp, 4
0x0017bd4f: mov      dword ptr [ebp - 0x10], eax
0x0017bd52: test     eax, eax
0x0017bd54: mov      byte ptr [ebp - 4], 0x4c
0x0017bd58: je       0x17bd80
0x0017bd5a: push     0x800
0x0017bd5f: push     0x683598  ; 'selslide.pcx'
0x0017bd64: push     0x185
0x0017bd69: push     0x7e
0x0017bd6b: push     0x13
0x0017bd6d: push     0x85
0x0017bd72: push     0x2da
0x0017bd77: mov      ecx, eax
0x0017bd79: call     0x4ffa0
0x0017bd7e: jmp      0x17bd82
0x0017bd82: lea      ecx, [ebp + 8]
0x0017bd85: mov      byte ptr [ebp - 4], 6
0x0017bd89: mov      dword ptr [ebp + 8], eax
0x0017bd8c: mov      eax, dword ptr [esi + 8]
0x0017bd8f: push     ecx
0x0017bd90: push     1
0x0017bd92: push     eax
0x0017bd93: mov      ecx, esi
0x0017bd95: call     0x1fe2d0
0x0017bd9a: mov      edx, dword ptr [ebx + 0x1838]
0x0017bda0: lea      ecx, [ebp + 8]
0x0017bda3: mov      dword ptr [ebp + 8], edx
0x0017bda6: mov      eax, dword ptr [esi + 8]
0x0017bda9: push     ecx
0x0017bdaa: push     1
0x0017bdac: push     eax
0x0017bdad: mov      ecx, esi
0x0017bdaf: call     0x1fe2d0
0x0017bdb4: mov      edx, dword ptr [ebx + 0x1848]
0x0017bdba: lea      ecx, [ebp + 8]
0x0017bdbd: mov      dword ptr [ebp + 8], edx
0x0017bdc0: mov      eax, dword ptr [esi + 8]
0x0017bdc3: push     ecx
0x0017bdc4: push     1
0x0017bdc6: push     eax
0x0017bdc7: mov      ecx, esi
0x0017bdc9: call     0x1fe2d0
0x0017bdce: mov      edx, dword ptr [ebx + 0x1858]
0x0017bdd4: lea      ecx, [ebp + 8]
0x0017bdd7: mov      dword ptr [ebp + 8], edx
0x0017bdda: mov      eax, dword ptr [esi + 8]
0x0017bddd: push     ecx
0x0017bdde: push     1
0x0017bde0: push     eax
0x0017bde1: mov      ecx, esi
0x0017bde3: call     0x1fe2d0
0x0017bde8: mov      edx, dword ptr [ebx + 0x184c]
0x0017bdee: lea      ecx, [ebp + 8]
0x0017bdf1: mov      dword ptr [ebp + 8], edx
0x0017bdf4: mov      eax, dword ptr [esi + 8]
0x0017bdf7: push     ecx
0x0017bdf8: push     1
0x0017bdfa: push     eax
0x0017bdfb: mov      ecx, esi
0x0017bdfd: call     0x1fe2d0
0x0017be02: mov      edx, dword ptr [ebx + 0x1850]
0x0017be08: lea      ecx, [ebp + 8]
0x0017be0b: mov      dword ptr [ebp + 8], edx
0x0017be0e: mov      eax, dword ptr [esi + 8]
0x0017be11: push     ecx
0x0017be12: push     1
0x0017be14: push     eax
0x0017be15: mov      ecx, esi
0x0017be17: call     0x1fe2d0
0x0017be1c: mov      ecx, dword ptr [ebx + 0x1858]
0x0017be22: push     1
0x0017be24: mov      edx, dword ptr [ecx]
0x0017be26: call     dword ptr [edx + 0x38]
0x0017be29: mov      al, byte ptr [ebx + 0x64]
0x0017be2c: test     al, al
0x0017be2e: jne      0x17be3f
0x0017be30: mov      cl, byte ptr [ebx + 0x65]
0x0017be33: test     cl, cl
0x0017be35: je       0x17be55
0x0017be37: test     al, al
0x0017be39: je       0x17c2db
0x0017be3f: mov      eax, dword ptr [0x69959c]
0x0017be44: test     eax, eax
0x0017be46: jne      0x17be55
0x0017be48: cmp      dword ptr [0x698a40], 3
0x0017be4f: jne      0x17c2db
0x0017be55: mov      ecx, ebx
0x0017be57: call     0x17d440
0x0017be5c: mov      al, byte ptr [ebx + 0x64]
0x0017be5f: push     0x70
0x0017be61: test     al, al
0x0017be63: je       0x17becb
0x0017be65: call     0x217492
0x0017be6a: add      esp, 4
0x0017be6d: mov      dword ptr [ebp - 0x10], eax
0x0017be70: test     eax, eax
0x0017be72: mov      byte ptr [ebp - 4], 0x4d
0x0017be76: je       0x17beba
0x0017be78: mov      ecx, dword ptr [0x6a5dc4]
0x0017be7e: push     4
0x0017be80: push     2
0x0017be82: push     0x1f
0x0017be84: mov      edx, dword ptr [ecx + 0x20]
0x0017be87: push     0
0x0017be89: push     1
0x0017be8b: push     0
0x0017be8d: mov      edx, dword ptr [edx + 0xa40]
0x0017be93: push     0x65f2f8  ; 'smalfont.fnt'
0x0017be98: push     edx
0x0017be99: push     0x68358c  ; 'gspbutt.def'
0x0017be9e: push     0x80
0x0017bea3: push     0x14
0x0017bea5: push     0xc8
0x0017beaa: push     0x51
0x0017beac: push     0x19e
0x0017beb1: mov      ecx, eax
0x0017beb3: call     0x56730
0x0017beb8: jmp      0x17bebc
0x0017bebc: mov      byte ptr [ebp - 4], 6
0x0017bec0: mov      dword ptr [ebp + 8], eax
0x0017bec3: mov      eax, dword ptr [esi + 8]
0x0017bec6: lea      edx, [ebp + 8]
0x0017bec9: jmp      0x17bf2f
0x0017bf2f: push     edx
0x0017bf30: push     1
0x0017bf32: push     eax
0x0017bf33: mov      ecx, esi
0x0017bf35: call     0x1fe2d0
0x0017bf3a: push     0x70
0x0017bf3c: call     0x217492
0x0017bf41: add      esp, 4
0x0017bf44: mov      dword ptr [ebp + 8], eax
0x0017bf47: test     eax, eax
0x0017bf49: mov      byte ptr [ebp - 4], 0x4f
0x0017bf4d: je       0x17bf94
0x0017bf4f: mov      ecx, dword ptr [0x6a5dc4]
0x0017bf55: push     4
0x0017bf57: push     2
0x0017bf59: push     0x1e
0x0017bf5b: mov      edx, dword ptr [ecx + 0x20]
0x0017bf5e: push     0
0x0017bf60: push     1
0x0017bf62: push     0
0x0017bf64: mov      edx, dword ptr [edx + 0x7d8]
0x0017bf6a: push     0x65f2f8  ; 'smalfont.fnt'
0x0017bf6f: push     edx
0x0017bf70: push     0x68358c  ; 'gspbutt.def'
0x0017bf75: push     0x81
0x0017bf7a: push     0x14
0x0017bf7c: push     0xc8
0x0017bf81: push     0x1fd
0x0017bf86: push     0x19e
0x0017bf8b: mov      ecx, eax
0x0017bf8d: call     0x56730
0x0017bf92: jmp      0x17bf96
0x0017bf96: mov      dword ptr [ebp - 0x14], eax
0x0017bf99: lea      eax, [esi + 8]
0x0017bf9c: mov      byte ptr [ebp - 4], 6
0x0017bfa0: mov      dword ptr [ebp + 8], eax
0x0017bfa3: mov      edi, dword ptr [eax]
0x0017bfa5: mov      edx, dword ptr [esi + 0xc]
0x0017bfa8: sub      edx, edi
0x0017bfaa: sar      edx, 2
0x0017bfad: cmp      edx, 1
0x0017bfb0: jae      0x17c06d
0x0017bfb6: mov      ecx, esi
0x0017bfb8: call     0x14d2b0
0x0017bfbd: cmp      eax, 1
0x0017bfc0: jbe      0x17bfce
0x0017bfc2: mov      ecx, esi
0x0017bfc4: call     0x14d2b0
0x0017bfc9: mov      dword ptr [ebp - 0x18], eax
0x0017bfcc: jmp      0x17bfd5
0x0017bfd5: mov      ecx, esi
0x0017bfd7: call     0x14d2b0
0x0017bfdc: add      eax, dword ptr [ebp - 0x18]
0x0017bfdf: mov      dword ptr [ebp - 0x1c], eax
0x0017bfe2: jns      0x17bfe6
0x0017bfe4: xor      eax, eax
0x0017bfe6: shl      eax, 2
0x0017bfe9: push     eax
0x0017bfea: call     0x217492
0x0017bfef: add      esp, 4
0x0017bff2: mov      dword ptr [ebp - 0x18], eax
0x0017bff5: mov      ecx, dword ptr [esi + 4]
0x0017bff8: push     eax
0x0017bff9: push     edi
0x0017bffa: push     ecx
0x0017bffb: mov      ecx, esi
0x0017bffd: call     0x232e0
0x0017c002: lea      edx, [ebp - 0x14]
0x0017c005: mov      ecx, esi
0x0017c007: push     edx
0x0017c008: push     1
0x0017c00a: push     eax
0x0017c00b: mov      dword ptr [ebp - 0x10], eax
0x0017c00e: call     0x23310
0x0017c013: mov      eax, dword ptr [ebp - 0x10]
0x0017c016: mov      ecx, dword ptr [ebp + 8]
0x0017c019: add      eax, 4
0x0017c01c: mov      edx, dword ptr [ecx]
0x0017c01e: push     eax
0x0017c01f: push     edx
0x0017c020: push     edi
0x0017c021: mov      ecx, esi
0x0017c023: call     0x232e0
0x0017c028: mov      eax, dword ptr [ebp + 8]
0x0017c02b: mov      edx, dword ptr [esi + 4]
0x0017c02e: lea      edi, [esi + 4]
0x0017c031: mov      ecx, dword ptr [eax]
0x0017c033: push     ecx
0x0017c034: push     edx
0x0017c035: mov      ecx, esi
0x0017c037: call     0xaf240
0x0017c03c: mov      eax, dword ptr [edi]
0x0017c03e: push     eax
0x0017c03f: call     0x20b0f0
0x0017c044: mov      eax, dword ptr [ebp - 0x1c]
0x0017c047: mov      ecx, dword ptr [ebp - 0x18]
0x0017c04a: add      esp, 4
0x0017c04d: lea      edx, [ecx + eax*4]
0x0017c050: mov      ecx, esi
0x0017c052: mov      dword ptr [esi + 0xc], edx
0x0017c055: call     0x14d2b0
0x0017c05a: mov      ecx, dword ptr [ebp - 0x18]
0x0017c05d: mov      edx, dword ptr [ebp + 8]
0x0017c060: lea      eax, [ecx + eax*4 + 4]
0x0017c064: mov      dword ptr [edx], eax
0x0017c066: mov      dword ptr [edi], ecx
0x0017c068: jmp      0x17c100
0x0017c100: mov      eax, dword ptr [0x69928c]
0x0017c105: mov      eax, dword ptr [eax]
0x0017c107: cmp      eax, 1
0x0017c10a: je       0x17c115
0x0017c10c: cmp      eax, 3
0x0017c10f: jne      0x17c2db
0x0017c115: push     0x70
0x0017c117: call     0x217492
0x0017c11c: add      esp, 4
0x0017c11f: mov      dword ptr [ebp + 8], eax
0x0017c122: test     eax, eax
0x0017c124: mov      byte ptr [ebp - 4], 0x50
0x0017c128: je       0x17c16c
0x0017c12a: mov      ecx, dword ptr [0x6a5dc4]
0x0017c130: push     4
0x0017c132: push     2
0x0017c134: push     0x13
0x0017c136: mov      edx, dword ptr [ecx + 0x20]
0x0017c139: push     0
0x0017c13b: push     1
0x0017c13d: push     0
0x0017c13f: mov      edx, dword ptr [edx + 0xbe0]
0x0017c145: push     0x65f2f8  ; 'smalfont.fnt'
0x0017c14a: push     edx
0x0017c14b: push     0x68358c  ; 'gspbutt.def'
0x0017c150: push     0x82
0x0017c155: push     0x14
0x0017c157: push     0xc8
0x0017c15c: push     0x69
0x0017c15e: push     0x19e
0x0017c163: mov      ecx, eax
0x0017c165: call     0x56730
0x0017c16a: jmp      0x17c16e
0x0017c16e: lea      esi, [ebx + 0x30]
0x0017c171: mov      dword ptr [ebp - 0x14], eax
0x0017c174: mov      byte ptr [ebp - 4], 6
0x0017c178: lea      eax, [esi + 8]
0x0017c17b: mov      dword ptr [ebp + 8], eax
0x0017c17e: mov      edx, dword ptr [esi + 0xc]
0x0017c181: mov      edi, dword ptr [eax]
0x0017c183: sub      edx, edi
0x0017c185: sar      edx, 2
0x0017c188: cmp      edx, 1
0x0017c18b: jae      0x17c248
0x0017c191: mov      ecx, esi
0x0017c193: call     0x14d2b0
0x0017c198: cmp      eax, 1
0x0017c19b: jbe      0x17c1a9
0x0017c19d: mov      ecx, esi
0x0017c19f: call     0x14d2b0
0x0017c1a4: mov      dword ptr [ebp - 0x18], eax
0x0017c1a7: jmp      0x17c1b0
0x0017c1b0: mov      ecx, esi
0x0017c1b2: call     0x14d2b0
0x0017c1b7: add      eax, dword ptr [ebp - 0x18]
0x0017c1ba: mov      dword ptr [ebp - 0x1c], eax
0x0017c1bd: jns      0x17c1c1
0x0017c1bf: xor      eax, eax
0x0017c1c1: shl      eax, 2
0x0017c1c4: push     eax
0x0017c1c5: call     0x217492
0x0017c1ca: add      esp, 4
0x0017c1cd: mov      dword ptr [ebp - 0x18], eax
0x0017c1d0: mov      ecx, dword ptr [esi + 4]
0x0017c1d3: push     eax
0x0017c1d4: push     edi
0x0017c1d5: push     ecx
0x0017c1d6: mov      ecx, esi
0x0017c1d8: call     0x232e0
0x0017c1dd: lea      edx, [ebp - 0x14]
0x0017c1e0: mov      ecx, esi
0x0017c1e2: push     edx
0x0017c1e3: push     1
0x0017c1e5: push     eax
0x0017c1e6: mov      dword ptr [ebp - 0x10], eax
0x0017c1e9: call     0x23310
0x0017c1ee: mov      eax, dword ptr [ebp - 0x10]
0x0017c1f1: mov      ecx, dword ptr [ebp + 8]
0x0017c1f4: add      eax, 4
0x0017c1f7: mov      edx, dword ptr [ecx]
0x0017c1f9: push     eax
0x0017c1fa: push     edx
0x0017c1fb: push     edi
0x0017c1fc: mov      ecx, esi
0x0017c1fe: call     0x232e0
0x0017c203: mov      eax, dword ptr [ebp + 8]
0x0017c206: mov      edx, dword ptr [esi + 4]
0x0017c209: lea      edi, [esi + 4]
0x0017c20c: mov      ecx, dword ptr [eax]
0x0017c20e: push     ecx
0x0017c20f: push     edx
0x0017c210: mov      ecx, esi
0x0017c212: call     0xaf240
0x0017c217: mov      eax, dword ptr [edi]
0x0017c219: push     eax
0x0017c21a: call     0x20b0f0
0x0017c21f: mov      eax, dword ptr [ebp - 0x1c]
0x0017c222: mov      ecx, dword ptr [ebp - 0x18]
0x0017c225: add      esp, 4
0x0017c228: lea      edx, [ecx + eax*4]
0x0017c22b: mov      ecx, esi
0x0017c22d: mov      dword ptr [esi + 0xc], edx
0x0017c230: call     0x14d2b0
0x0017c235: mov      ecx, dword ptr [ebp - 0x18]
0x0017c238: mov      edx, dword ptr [ebp + 8]
0x0017c23b: lea      eax, [ecx + eax*4 + 4]
0x0017c23f: mov      dword ptr [edx], eax
0x0017c241: mov      dword ptr [edi], ecx
0x0017c243: jmp      0x17c2db
0x0017c2db: push     0x68
0x0017c2dd: call     0x217492
0x0017c2e2: add      esp, 4
0x0017c2e5: mov      dword ptr [ebp + 8], eax
0x0017c2e8: test     eax, eax
0x0017c2ea: mov      byte ptr [ebp - 4], 0x51
0x0017c2ee: je       0x17c318
0x0017c2f0: push     2
0x0017c2f2: push     0
0x0017c2f4: push     0
0x0017c2f6: push     1
0x0017c2f8: push     0
0x0017c2fa: push     0x660e5c  ; 'gspbut3.def'
0x0017c2ff: push     0x6b
0x0017c301: push     0x2e
0x0017c303: push     0x1e
0x0017c305: push     0x1c8
0x0017c30a: push     0x1fa
0x0017c30f: mov      ecx, eax
0x0017c311: call     0x55bd0
0x0017c316: jmp      0x17c31a
0x0017c31a: lea      esi, [ebx + 0x30]
0x0017c31d: mov      dword ptr [ebp - 0x14], eax
0x0017c320: mov      byte ptr [ebp - 4], 6
0x0017c324: lea      eax, [esi + 8]
0x0017c327: mov      dword ptr [ebp + 8], eax
0x0017c32a: mov      edi, dword ptr [eax]
0x0017c32c: mov      eax, dword ptr [esi + 0xc]
0x0017c32f: sub      eax, edi
0x0017c331: sar      eax, 2
0x0017c334: cmp      eax, 1
0x0017c337: jae      0x17c3f8
0x0017c33d: mov      ecx, esi
0x0017c33f: call     0x14d2b0
0x0017c344: cmp      eax, 1
0x0017c347: jbe      0x17c355
0x0017c349: mov      ecx, esi
0x0017c34b: call     0x14d2b0
0x0017c350: mov      dword ptr [ebp - 0x18], eax
0x0017c353: jmp      0x17c35c
0x0017c35c: mov      ecx, esi
0x0017c35e: call     0x14d2b0
0x0017c363: add      eax, dword ptr [ebp - 0x18]
0x0017c366: mov      dword ptr [ebp - 0x1c], eax
0x0017c369: jns      0x17c36d
0x0017c36b: xor      eax, eax
0x0017c36d: lea      ecx, [eax*4]
0x0017c374: push     ecx
0x0017c375: call     0x217492
0x0017c37a: add      esp, 4
0x0017c37d: mov      dword ptr [ebp - 0x18], eax
0x0017c380: mov      edx, dword ptr [esi + 4]
0x0017c383: mov      ecx, esi
0x0017c385: push     eax
0x0017c386: push     edi
0x0017c387: push     edx
0x0017c388: call     0x232e0
0x0017c38d: lea      ecx, [ebp - 0x14]
0x0017c390: mov      dword ptr [ebp - 0x10], eax
0x0017c393: push     ecx
0x0017c394: push     1
0x0017c396: push     eax
0x0017c397: mov      ecx, esi
0x0017c399: call     0x23310
0x0017c39e: mov      edx, dword ptr [ebp - 0x10]
0x0017c3a1: mov      eax, dword ptr [ebp + 8]
0x0017c3a4: add      edx, 4
0x0017c3a7: mov      ecx, dword ptr [eax]
0x0017c3a9: push     edx
0x0017c3aa: push     ecx
0x0017c3ab: push     edi
0x0017c3ac: mov      ecx, esi
0x0017c3ae: call     0x232e0
0x0017c3b3: mov      edx, dword ptr [ebp + 8]
0x0017c3b6: mov      ecx, dword ptr [esi + 4]
0x0017c3b9: lea      edi, [esi + 4]
0x0017c3bc: mov      eax, dword ptr [edx]
0x0017c3be: push     eax
0x0017c3bf: push     ecx
0x0017c3c0: mov      ecx, esi
0x0017c3c2: call     0xaf240
0x0017c3c7: mov      eax, dword ptr [edi]
0x0017c3c9: push     eax
0x0017c3ca: call     0x20b0f0
0x0017c3cf: mov      edx, dword ptr [ebp - 0x1c]
0x0017c3d2: mov      eax, dword ptr [ebp - 0x18]
0x0017c3d5: add      esp, 4
0x0017c3d8: lea      ecx, [eax + edx*4]
0x0017c3db: mov      dword ptr [esi + 0xc], ecx
0x0017c3de: mov      ecx, esi
0x0017c3e0: call     0x14d2b0
0x0017c3e5: mov      ecx, dword ptr [ebp - 0x18]
0x0017c3e8: lea      edx, [ecx + eax*4 + 4]
0x0017c3ec: mov      eax, dword ptr [ebp + 8]
0x0017c3ef: mov      dword ptr [eax], edx
0x0017c3f1: mov      dword ptr [edi], ecx
0x0017c3f3: jmp      0x17c48b
0x0017c48b: push     0x68
0x0017c48d: call     0x217492
0x0017c492: add      esp, 4
0x0017c495: mov      dword ptr [ebp - 0x10], eax
0x0017c498: test     eax, eax
0x0017c49a: mov      byte ptr [ebp - 4], 0x52
0x0017c49e: je       0x17c4c8
0x0017c4a0: push     2
0x0017c4a2: push     0
0x0017c4a4: push     0
0x0017c4a6: push     1
0x0017c4a8: push     0
0x0017c4aa: push     0x660e50  ; 'gspbut4.def'
0x0017c4af: push     0x6c
0x0017c4b1: push     0x2e
0x0017c4b3: push     0x1e
0x0017c4b5: push     0x1c8
0x0017c4ba: push     0x21a
0x0017c4bf: mov      ecx, eax
0x0017c4c1: call     0x55bd0
0x0017c4c6: jmp      0x17c4ca
0x0017c4ca: mov      dword ptr [ebp + 8], eax
0x0017c4cd: lea      esi, [ebx + 0x30]
0x0017c4d0: lea      eax, [ebp + 8]
0x0017c4d3: mov      ecx, esi
0x0017c4d5: push     eax
0x0017c4d6: mov      byte ptr [ebp - 4], 6
0x0017c4da: call     0x14c900
0x0017c4df: push     0x68
0x0017c4e1: call     0x217492
0x0017c4e6: add      esp, 4
0x0017c4e9: mov      dword ptr [ebp - 0x10], eax
0x0017c4ec: test     eax, eax
0x0017c4ee: mov      byte ptr [ebp - 4], 0x53
0x0017c4f2: je       0x17c51c
0x0017c4f4: push     2
0x0017c4f6: push     0
0x0017c4f8: push     0
0x0017c4fa: push     1
0x0017c4fc: push     0
0x0017c4fe: push     0x660e44  ; 'gspbut5.def'
0x0017c503: push     0x6d
0x0017c505: push     0x2e
0x0017c507: push     0x1e
0x0017c509: push     0x1c8
0x0017c50e: push     0x23a
0x0017c513: mov      ecx, eax
0x0017c515: call     0x55bd0
0x0017c51a: jmp      0x17c51e
0x0017c51e: lea      ecx, [ebp + 8]
0x0017c521: mov      byte ptr [ebp - 4], 6
0x0017c525: push     ecx
0x0017c526: mov      ecx, esi
0x0017c528: mov      dword ptr [ebp + 8], eax
0x0017c52b: call     0x14c900
0x0017c530: push     0x68
0x0017c532: call     0x217492
0x0017c537: add      esp, 4
0x0017c53a: mov      dword ptr [ebp - 0x10], eax
0x0017c53d: test     eax, eax
0x0017c53f: mov      byte ptr [ebp - 4], 0x54
0x0017c543: je       0x17c56d
0x0017c545: push     2
0x0017c547: push     0
0x0017c549: push     0
0x0017c54b: push     1
0x0017c54d: push     0
0x0017c54f: push     0x660e38  ; 'gspbut6.def'
0x0017c554: push     0x6e
0x0017c556: push     0x2e
0x0017c558: push     0x1e
0x0017c55a: push     0x1c8
0x0017c55f: push     0x25a
0x0017c564: mov      ecx, eax
0x0017c566: call     0x55bd0
0x0017c56b: jmp      0x17c56f
0x0017c56f: lea      edx, [ebp + 8]
0x0017c572: mov      ecx, esi
0x0017c574: push     edx
0x0017c575: mov      byte ptr [ebp - 4], 6
0x0017c579: mov      dword ptr [ebp + 8], eax
0x0017c57c: call     0x14c900
0x0017c581: push     0x68
0x0017c583: call     0x217492
0x0017c588: add      esp, 4
0x0017c58b: mov      dword ptr [ebp - 0x10], eax
0x0017c58e: test     eax, eax
0x0017c590: mov      byte ptr [ebp - 4], 0x55
0x0017c594: je       0x17c5be
0x0017c596: push     2
0x0017c598: push     0
0x0017c59a: push     0
0x0017c59c: push     1
0x0017c59e: push     0
0x0017c5a0: push     0x660e2c  ; 'gspbut7.def'
0x0017c5a5: push     0x6f
0x0017c5a7: push     0x2e
0x0017c5a9: push     0x1e
0x0017c5ab: push     0x1c8
0x0017c5b0: push     0x27a
0x0017c5b5: mov      ecx, eax
0x0017c5b7: call     0x55bd0
0x0017c5bc: jmp      0x17c5c0
0x0017c5c0: mov      dword ptr [ebp + 8], eax
0x0017c5c3: lea      eax, [ebp + 8]
0x0017c5c6: push     eax
0x0017c5c7: lea      ecx, [ebx + 0x30]
0x0017c5ca: mov      byte ptr [ebp - 4], 6
0x0017c5ce: call     0x14c900
0x0017c5d3: mov      al, byte ptr [ebx + 0x64]
0x0017c5d6: mov      esi, 0x675574  ; 'scnrbeg.def'
0x0017c5db: test     al, al
0x0017c5dd: mov      edi, 0x30
0x0017c5e2: je       0x17c5f0
0x0017c5e4: mov      esi, 0x683580  ; 'scnrlod.def'
0x0017c5e9: mov      edi, 0x26
0x0017c5ee: jmp      0x17c601
0x0017c601: push     0x68
0x0017c603: call     0x217492
0x0017c608: add      esp, 4
0x0017c60b: mov      dword ptr [ebp + 8], eax
0x0017c60e: test     eax, eax
0x0017c610: mov      byte ptr [ebp - 4], 0x56
0x0017c614: je       0x17c641
0x0017c616: push     2
0x0017c618: push     edi
0x0017c619: push     0
0x0017c61b: push     1
0x0017c61d: push     0
0x0017c61f: push     esi
0x0017c620: push     0xba
0x0017c625: push     0x28
0x0017c627: push     0xa6
0x0017c62c: push     0x217
0x0017c631: push     0x19e
0x0017c636: mov      ecx, eax
0x0017c638: call     0x55bd0
0x0017c63d: mov      esi, eax
0x0017c63f: jmp      0x17c643
0x0017c643: mov      byte ptr [ebp - 4], 6
0x0017c647: mov      al, byte ptr [ebx + 0x64]
0x0017c64a: test     al, al
0x0017c64c: je       0x17c657
0x0017c64e: push     0x1c
0x0017c650: mov      ecx, esi
0x0017c652: call     0xe15e0
0x0017c657: lea      ecx, [ebp + 8]
0x0017c65a: lea      edi, [ebx + 0x30]
0x0017c65d: push     ecx
0x0017c65e: mov      ecx, edi
0x0017c660: mov      dword ptr [ebp + 8], esi
0x0017c663: call     0x14c900
0x0017c668: mov      al, byte ptr [0x69779c]
0x0017c66d: test     al, al
0x0017c66f: je       0x17c6dd
0x0017c671: mov      edx, dword ptr [0x699538]
0x0017c677: lea      ecx, [edx + 0x1f458]
0x0017c67d: call     0x17cb00
0x0017c682: cmp      byte ptr [eax], 0
0x0017c685: jne      0x17c734
0x0017c68b: push     0x68
0x0017c68d: call     0x217492
0x0017c692: add      esp, 4
0x0017c695: mov      dword ptr [ebp - 0x10], eax
0x0017c698: test     eax, eax
0x0017c69a: mov      byte ptr [ebp - 4], 0x57
0x0017c69e: je       0x17c6ce
0x0017c6a0: push     2
0x0017c6a2: push     1
0x0017c6a4: push     0
0x0017c6a6: push     1
0x0017c6a8: push     0
0x0017c6aa: push     0x675564  ; 'scnrback.def'
0x0017c6af: push     0xbc
0x0017c6b4: push     0x28
0x0017c6b6: push     0xa6
0x0017c6bb: push     0x217
0x0017c6c0: push     0x248
0x0017c6c5: mov      ecx, eax
0x0017c6c7: call     0x55bd0
0x0017c6cc: jmp      0x17c6d0
0x0017c6d0: mov      dword ptr [ebp + 8], eax
0x0017c6d3: lea      eax, [ebp + 8]
0x0017c6d6: mov      byte ptr [ebp - 4], 6
0x0017c6da: push     eax
0x0017c6db: jmp      0x17c72d
0x0017c72d: mov      ecx, edi
0x0017c72f: call     0x14c900
0x0017c734: mov      ecx, edi
0x0017c736: call     0xe6750
0x0017c73b: mov      ecx, edi
0x0017c73d: mov      esi, eax
0x0017c73f: call     0xe6760
0x0017c744: cmp      esi, eax
0x0017c746: je       0x17c76d
0x0017c748: mov      eax, dword ptr [esi]
0x0017c74a: test     eax, eax
0x0017c74c: je       0x17c75a
0x0017c74e: push     -1
0x0017c750: push     eax
0x0017c751: mov      ecx, ebx
0x0017c753: call     0x1ff270
0x0017c758: jmp      0x17c75f
0x0017c75f: mov      ecx, edi
0x0017c761: add      esi, 4
0x0017c764: call     0xe6760
0x0017c769: cmp      esi, eax
0x0017c76b: jne      0x17c748
0x0017c76d: mov      ecx, 0x683568  ; 'ScSelC.def'
0x0017c772: call     0x15c9c0
0x0017c777: mov      ecx, 0x683224  ; 'scnrvict.def'
0x0017c77c: mov      dword ptr [ebx + 0x6c], eax
0x0017c77f: call     0x15c9c0
0x0017c784: mov      ecx, 0x683214  ; 'scnrloss.def'
0x0017c789: mov      dword ptr [ebx + 0x70], eax
0x0017c78c: call     0x15c9c0
0x0017c791: mov      ecx, 0x65f318  ; 'itpa.def'
0x0017c796: mov      dword ptr [ebx + 0x74], eax
0x0017c799: call     0x15c9c0
0x0017c79e: mov      ecx, 0x679d90  ; 'un44.def'
0x0017c7a3: mov      dword ptr [ebx + 0x78], eax
0x0017c7a6: call     0x15c9c0
0x0017c7ab: mov      dword ptr [ebx + 0x80], eax
0x0017c7b1: xor      edi, edi
0x0017c7b3: mov      esi, 0xc8
0x0017c7b8: mov      edx, dword ptr [0x67dce8]
0x0017c7be: mov      ecx, dword ptr [edx + edi + 0x30]
0x0017c7c2: call     0x15aa10
0x0017c7c7: mov      dword ptr [esi + ebx], eax
0x0017c7ca: add      esi, 4
0x0017c7cd: add      edi, 0x5c
0x0017c7d0: cmp      esi, 0x354
0x0017c7d6: jl       0x17c7b8
0x0017c7d8: mov      ecx, 0x68355c  ; 'hpsrand.pcx'
0x0017c7dd: call     0x15aa10
0x0017c7e2: mov      ecx, 0x683204  ; 'ScnrStar.def'
0x0017c7e7: mov      dword ptr [ebx + 0x354], eax
0x0017c7ed: call     0x15c9c0
0x0017c7f2: mov      dword ptr [ebx + 0x7c], eax
0x0017c7f5: xor      edi, edi
0x0017c7f7: mov      esi, 0x88
0x0017c7fc: movsx    eax, byte ptr [edi + 0x641af0]  ; 'rbygopts'
0x0017c803: push     eax
0x0017c804: lea      ecx, [ebp - 0xc0]
0x0017c80a: push     0x6831e8  ; 'adop%cpnl.pcx'
0x0017c80f: push     ecx
0x0017c810: call     0x2179de
0x0017c815: add      esp, 0xc
0x0017c818: lea      ecx, [ebp - 0xc0]
0x0017c81e: call     0x15aa10
0x0017c823: mov      dword ptr [esi + ebx + 0x20], eax
0x0017c827: lea      eax, [ebp - 0xc0]
0x0017c82d: movsx    edx, byte ptr [edi + 0x641af0]  ; 'rbygopts'
0x0017c834: push     edx
0x0017c835: push     0x6831d8  ; 'adopflg%c.pcx'
0x0017c83a: push     eax
0x0017c83b: call     0x2179de
0x0017c840: add      esp, 0xc
0x0017c843: lea      ecx, [ebp - 0xc0]
0x0017c849: call     0x15aa10
0x0017c84e: mov      dword ptr [esi + ebx], eax
0x0017c851: add      esi, 4
0x0017c854: inc      edi
0x0017c855: cmp      esi, 0xa8
0x0017c85b: jl       0x17c7fc
0x0017c85d: mov      ecx, 0x68354c  ; 'hpsrand0.pcx'
0x0017c862: call     0x15aa10
0x0017c867: mov      ecx, 0x68353c  ; 'hpsrand1.pcx'
0x0017c86c: mov      dword ptr [ebx + 0x35c], eax
0x0017c872: call     0x15aa10
0x0017c877: mov      ecx, 0x6831c8  ; 'hpsrand6.pcx'
0x0017c87c: mov      dword ptr [ebx + 0x360], eax
0x0017c882: call     0x15aa10
0x0017c887: xor      esi, esi
0x0017c889: mov      dword ptr [ebx + 0x368], eax
0x0017c88f: mov      dword ptr [ebx + 0x370], esi
0x0017c895: mov      dword ptr [ebx + 0x374], esi
0x0017c89b: mov      al, byte ptr [ebx + 0x64]
0x0017c89e: test     al, al
0x0017c8a0: je       0x17c8ab
0x0017c8a2: mov      ecx, ebx
0x0017c8a4: call     0x17f600
0x0017c8a9: jmp      0x17c8b9
0x0017c8b9: call     0x177b90
0x0017c8be: lea      ecx, [ebx + 0x1050]
0x0017c8c4: call     0x18eda0
0x0017c8c9: test     eax, eax
0x0017c8cb: ja       0x17c8d4
0x0017c8cd: mov      al, byte ptr [ebx + 0x65]
0x0017c8d0: test     al, al
0x0017c8d2: je       0x17c8db
0x0017c8d4: mov      ecx, ebx
0x0017c8d6: call     0x186330
0x0017c8db: mov      ecx, ebx
0x0017c8dd: call     0x184f10
0x0017c8e2: cmp      dword ptr [0x69959c], esi
0x0017c8e8: je       0x17c91b
0x0017c8ea: mov      al, byte ptr [ebx + 0x65]
0x0017c8ed: test     al, al
0x0017c8ef: jne      0x17c91b
0x0017c8f1: push     0x20
0x0017c8f3: call     0x217492
0x0017c8f8: add      esp, 4
0x0017c8fb: mov      dword ptr [ebp + 8], eax
0x0017c8fe: cmp      eax, esi
0x0017c900: mov      byte ptr [ebp - 4], 0x59
0x0017c904: je       0x17c90f
0x0017c906: mov      ecx, eax
0x0017c908: call     0x17cd30
0x0017c90d: jmp      0x17c911
0x0017c911: mov      byte ptr [ebp - 4], 6
0x0017c915: mov      dword ptr [ebx + 0x388], eax
0x0017c91b: mov      al, byte ptr [ebx + 0x64]
0x0017c91e: test     al, al
0x0017c920: je       0x17c92a
0x0017c922: cmp      dword ptr [0x69959c], esi
0x0017c928: je       0x17c931
0x0017c92a: mov      al, byte ptr [ebx + 0x65]
0x0017c92d: test     al, al
0x0017c92f: je       0x17c93b
0x0017c931: push     esi
0x0017c932: mov      ecx, ebx
0x0017c934: call     0x180d40
0x0017c939: jmp      0x17c950
0x0017c950: mov      ecx, ebx
0x0017c952: call     0x17fe60
0x0017c957: cmp      dword ptr [0x69959c], esi
0x0017c95d: je       0x17c98d
0x0017c95f: mov      al, byte ptr [ebx + 0x65]
0x0017c962: test     al, al
0x0017c964: jne      0x17c98d
0x0017c966: push     esi
0x0017c967: mov      ecx, ebx
0x0017c969: call     0x18cd50
0x0017c96e: mov      ecx, ebx
0x0017c970: call     0x18b0c0
0x0017c975: test     al, al
0x0017c977: jne      0x17c98d
0x0017c979: push     0xba
0x0017c97e: mov      ecx, ebx
0x0017c980: call     0x1ff5b0
0x0017c985: mov      edx, dword ptr [eax]
0x0017c987: push     esi
0x0017c988: mov      ecx, eax
0x0017c98a: call     dword ptr [edx + 0x24]
0x0017c98d: push     0
0x0017c98f: push     0x159
0x0017c994: push     0x68
0x0017c996: push     0x6a6d08
0x0017c99b: mov      ecx, ebx
0x0017c99d: call     0x1ffea0
0x0017c9a2: mov      ecx, 8
0x0017c9a7: or       eax, 0xffffffff
0x0017c9aa: mov      edi, 0x69fb80
0x0017c9af: rep stosd dword ptr es:[edi], eax
0x0017c9b1: mov      ecx, 8
0x0017c9b6: mov      eax, 3
0x0017c9bb: mov      edi, 0x69fc54
0x0017c9c0: rep stosd dword ptr es:[edi], eax
0x0017c9c2: mov      al, byte ptr [ebx + 0x65]
0x0017c9c5: test     al, al
0x0017c9c7: je       0x17ca0b
0x0017c9c9: push     0xa0
0x0017c9ce: mov      ecx, ebx
0x0017c9d0: call     0x1ffa50
0x0017c9d5: mov      ecx, dword ptr [ebx + 0x380]
0x0017c9db: push     1
0x0017c9dd: mov      eax, dword ptr [ecx]
0x0017c9df: call     dword ptr [eax + 0x44]
0x0017c9e2: lea      ecx, [ebx + 0x1050]
0x0017c9e8: call     0x18eda0
0x0017c9ed: test     eax, eax
0x0017c9ef: jne      0x17c9fb
0x0017c9f1: push     eax
0x0017c9f2: push     -1
0x0017c9f4: mov      ecx, ebx
0x0017c9f6: call     0x1857d0
0x0017c9fb: push     0
0x0017c9fd: mov      ecx, ebx
0x0017c9ff: call     0x1855d0
0x0017ca04: mov      al, byte ptr [ebx + 0x65]
0x0017ca07: test     al, al
0x0017ca09: jne      0x17ca12
0x0017ca0b: mov      al, byte ptr [ebx + 0x64]
0x0017ca0e: test     al, al
0x0017ca10: je       0x17ca2e
0x0017ca12: mov      esi, 0x6b
0x0017ca17: push     esi
0x0017ca18: mov      ecx, ebx
0x0017ca1a: call     0x1ff5b0
0x0017ca1f: mov      edx, dword ptr [eax]
0x0017ca21: push     0
0x0017ca23: mov      ecx, eax
0x0017ca25: call     dword ptr [edx + 0x24]
0x0017ca28: inc      esi
0x0017ca29: cmp      esi, 0x6f
0x0017ca2c: jle      0x17ca17
0x0017ca2e: mov      ecx, dword ptr [ebp - 0xc]
0x0017ca31: pop      edi
0x0017ca32: mov      eax, ebx
0x0017ca34: pop      esi
0x0017ca35: pop      ebx
0x0017ca36: mov      dword ptr fs:[0], ecx
0x0017ca3d: mov      esp, ebp
0x0017ca3f: pop      ebp
0x0017ca40: ret      4
