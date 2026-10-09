; Function: HEADER_WRITER at 0x000bbda0
; Image base: 0x00400000
; Instructions: 177
; Direct calls: 11
; Indirect calls: 0

0x000bbda0: push     ebp
0x000bbda1: mov      ebp, esp
0x000bbda3: push     -1
0x000bbda5: push     0x62be52
0x000bbdaa: mov      eax, dword ptr fs:[0]
0x000bbdb0: push     eax
0x000bbdb1: mov      dword ptr fs:[0], esp
0x000bbdb8: sub      esp, 0x1c
0x000bbdbb: push     ebx
0x000bbdbc: push     esi
0x000bbdbd: lea      esi, [ecx + 0x10]
0x000bbdc0: lea      eax, [ebp - 0xd]
0x000bbdc3: push     edi
0x000bbdc4: mov      dword ptr [ebp - 0x18], ecx
0x000bbdc7: push     eax
0x000bbdc8: lea      ecx, [esi + 0x20]
0x000bbdcb: mov      dword ptr [ebp - 0x28], esi
0x000bbdce: call     0x34600  ; → sub_00034600
0x000bbdd3: lea      ecx, [esi + 0x30]
0x000bbdd6: mov      dword ptr [ebp - 4], 0
0x000bbddd: call     0xbc000  ; → sub_000bc000
0x000bbde2: lea      ecx, [esi + 0x7c]
0x000bbde5: call     0x5b7a0  ; → sub_0005b7a0
0x000bbdea: push     0x45a860
0x000bbdef: push     0x45a630
0x000bbdf4: push     8
0x000bbdf6: lea      ecx, [esi + 0xa0]
0x000bbdfc: push     0x44
0x000bbdfe: push     ecx
0x000bbdff: call     0x2181fa  ; → sub_002181fa
0x000bbe04: lea      edx, [ebp - 0xe]
0x000bbe07: lea      eax, [ebp - 0xf]
0x000bbe0a: push     edx
0x000bbe0b: push     eax
0x000bbe0c: lea      ecx, [esi + 0x2c0]
0x000bbe12: mov      byte ptr [ebp - 4], 1
0x000bbe16: call     0x5bb10  ; → sub_0005bb10
0x000bbe1b: mov      cl, byte ptr [ebp - 0xf]
0x000bbe1e: lea      ebx, [esi + 0x2d0]
0x000bbe24: push     0
0x000bbe26: mov      dword ptr [ebp - 4], 2
0x000bbe2d: mov      byte ptr [ebx], cl
0x000bbe2f: mov      ecx, ebx
0x000bbe31: call     0x4130  ; → sub_00004130
0x000bbe36: mov      dl, byte ptr [ebp - 0xf]
0x000bbe39: lea      ecx, [esi + 0x2e0]
0x000bbe3f: push     0
0x000bbe41: mov      byte ptr [ebp - 4], 3
0x000bbe45: mov      dword ptr [ebp - 0x14], ecx
0x000bbe48: mov      byte ptr [ecx], dl
0x000bbe4a: call     0x4130  ; → sub_00004130
0x000bbe4f: lea      ecx, [esi + 0x2f0]
0x000bbe55: push     0
0x000bbe57: mov      byte ptr [ebp - 4], 4
0x000bbe5b: call     0x5bd40  ; → sub_0005bd40
0x000bbe60: mov      edi, 0x691260
0x000bbe65: or       ecx, 0xffffffff
0x000bbe68: xor      eax, eax
0x000bbe6a: mov      dword ptr [esi], 0
0x000bbe70: mov      byte ptr [esi + 5], 0
0x000bbe74: mov      byte ptr [esi + 6], 0
0x000bbe78: mov      byte ptr [esi + 7], 0
0x000bbe7c: mov      byte ptr [esi + 8], 0
0x000bbe80: mov      byte ptr [esi + 9], 0
0x000bbe84: mov      byte ptr [esi + 0xa], 0
0x000bbe88: repne scasb al, byte ptr es:[edi]
0x000bbe8a: not      ecx
0x000bbe8c: dec      ecx
0x000bbe8d: push     ecx
0x000bbe8e: push     0x691260
0x000bbe93: mov      ecx, ebx
0x000bbe95: call     0x4180  ; → sub_00004180
0x000bbe9a: mov      edi, 0x691260
0x000bbe9f: or       ecx, 0xffffffff
0x000bbea2: xor      eax, eax
0x000bbea4: repne scasb al, byte ptr es:[edi]
0x000bbea6: not      ecx
0x000bbea8: dec      ecx
0x000bbea9: push     ecx
0x000bbeaa: mov      ecx, dword ptr [ebp - 0x14]
0x000bbead: push     0x691260
0x000bbeb2: call     0x4180  ; → sub_00004180
0x000bbeb7: mov      dword ptr [ebp - 4], 5
0x000bbebe: mov      eax, dword ptr [ebp - 0x18]
0x000bbec1: xor      ecx, ecx
0x000bbec3: lea      ebx, [eax + 0x314]
0x000bbec9: mov      esi, ebx
0x000bbecb: lea      eax, [ebx + 8]
0x000bbece: lea      edx, [ebx + 0x1a4]
0x000bbed4: sub      esi, eax
0x000bbed6: mov      dword ptr [ebp - 0x14], edx
0x000bbed9: add      esi, 0x30
0x000bbedc: mov      edx, ebx
0x000bbede: mov      dword ptr [ebp - 0x20], esi
0x000bbee1: mov      esi, ebx
0x000bbee3: sub      esi, eax
0x000bbee5: sub      edx, eax
0x000bbee7: add      esi, 0x198
0x000bbeed: mov      dword ptr [ebp - 0x1c], edx
0x000bbef0: mov      dword ptr [ebp - 0x24], esi
0x000bbef3: mov      esi, ebx
0x000bbef5: sub      esi, eax
0x000bbef7: add      esi, 0x1c4
0x000bbefd: mov      dword ptr [ebp - 0x28], esi
0x000bbf00: jmp      0xbbf05
0x000bbf05: lea      esi, [ebx + ecx + 8]
0x000bbf09: mov      eax, ecx
0x000bbf0b: mov      edi, 9
0x000bbf10: mov      byte ptr [edx + esi], cl
0x000bbf13: mov      byte ptr [esi], 0
0x000bbf16: cdq      
0x000bbf17: idiv     edi
0x000bbf19: mov      eax, dword ptr [ebp - 0x14]
0x000bbf1c: add      eax, 4
0x000bbf1f: mov      dword ptr [ebp - 0x14], eax
0x000bbf22: mov      dword ptr [eax - 0x198], edx
0x000bbf28: mov      edx, dword ptr [ebp - 0x20]
0x000bbf2b: mov      byte ptr [edx + esi], cl
0x000bbf2e: mov      edx, dword ptr [ebp - 0x24]
0x000bbf31: mov      byte ptr [edx + esi], cl
0x000bbf34: mov      edx, dword ptr [ebp - 0x28]
0x000bbf37: mov      dword ptr [eax - 4], 0xffffffff
0x000bbf3e: inc      ecx
0x000bbf3f: cmp      ecx, 8
0x000bbf42: mov      byte ptr [edx + esi], 3
0x000bbf46: jl       0xbbf02
0x000bbf48: mov      ecx, 0x3e
0x000bbf4d: xor      eax, eax
0x000bbf4f: lea      edi, [ebx + 0x39]
0x000bbf52: mov      byte ptr [ebx + 0x38], 0
0x000bbf56: mov      byte ptr [ebx + 0x1a3], 0xa
0x000bbf5d: rep stosd dword ptr es:[edi], eax
0x000bbf5f: stosw    word ptr es:[edi], ax
0x000bbf61: stosb    byte ptr es:[edi], al
0x000bbf62: mov      ecx, 0x19
0x000bbf67: xor      eax, eax
0x000bbf69: lea      edi, [ebx + 0x134]
0x000bbf6f: rep stosd dword ptr es:[edi], eax
0x000bbf71: mov      byte ptr [ebx + 0x1a0], al
0x000bbf77: mov      byte ptr [ebx + 0x1a1], al
0x000bbf7d: mov      byte ptr [ebx + 0x1a2], al
0x000bbf83: mov      ebx, dword ptr [ebp - 0x18]
0x000bbf86: lea      ecx, [ebx + 0x4e4]
0x000bbf8c: call     0x89040  ; → sub_00089040
0x000bbf91: mov      al, byte ptr [ebp - 0xf]
0x000bbf94: mov      edx, ebx
0x000bbf96: mov      byte ptr [ebx + 0x560], al
0x000bbf9c: xor      eax, eax
0x000bbf9e: xor      ecx, ecx
0x000bbfa0: mov      dword ptr [ebx + 0x564], eax
0x000bbfa6: mov      dword ptr [ebx + 0x568], eax
0x000bbfac: mov      dword ptr [ebx + 0x56c], eax
0x000bbfb2: mov      dword ptr [edx], ecx
0x000bbfb4: mov      edi, 0x677d38  ; "H3SVG"
0x000bbfb9: mov      dword ptr [edx + 4], ecx
0x000bbfbc: or       ecx, 0xffffffff
0x000bbfbf: repne scasb al, byte ptr es:[edi]
0x000bbfc1: not      ecx
0x000bbfc3: sub      edi, ecx
0x000bbfc5: mov      eax, ecx
0x000bbfc7: mov      esi, edi
0x000bbfc9: mov      edi, ebx
0x000bbfcb: shr      ecx, 2
0x000bbfce: rep movsd dword ptr es:[edi], dword ptr [esi]
0x000bbfd0: mov      ecx, eax
0x000bbfd2: mov      eax, ebx
0x000bbfd4: and      ecx, 3
0x000bbfd7: rep movsb byte ptr es:[edi], byte ptr [esi]
0x000bbfd9: mov      ecx, dword ptr [ebp - 0xc]
0x000bbfdc: pop      edi
0x000bbfdd: mov      dword ptr [ebx + 8], 0x2a
0x000bbfe4: pop      esi
0x000bbfe5: pop      ebx
0x000bbfe6: mov      dword ptr fs:[0], ecx
0x000bbfed: mov      esp, ebp
0x000bbfef: pop      ebp
0x000bbff0: ret      
