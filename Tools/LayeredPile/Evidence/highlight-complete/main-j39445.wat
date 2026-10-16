  (func $j39445 (type 9) (param i32 i32 i32 i32) (result i32)
    (local i32 i32 i32 i32 f32 f32 f32 f32 f32 f32 f64)
    global.get 0
    i32.const 464
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    i32.const 448
    i32.add
    local.get 2
    i32.const 0
    call $j1649 ;; 
    local.get 3
    f32.load offset=448
    local.set 8
    local.get 3
    f32.load offset=452
    local.set 9
    local.get 3
    f32.load offset=456
    local.set 10
    local.get 3
    i32.const 448
    i32.add
    local.get 2
    i32.const 0
    call $j1905 ;; 
    local.get 3
    f32.load offset=452
    local.set 11
    local.get 3
    f32.load offset=448
    local.set 12
    local.get 3
    f32.load offset=456
    local.set 13
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 10
    f32.store offset=440
    local.get 3
    local.get 3
    i32.load offset=440
    i32.store offset=216
    local.get 3
    local.get 9
    f32.store offset=436
    local.get 3
    local.get 8
    f32.store offset=432
    local.get 3
    i32.load offset=432
    local.set 5
    local.get 3
    i32.load offset=436
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=208
    local.get 3
    local.get 4
    i32.store offset=212
    local.get 2
    local.get 3
    i32.load offset=216
    i32.store offset=24
    local.get 3
    i32.load offset=208
    local.set 4
    local.get 3
    i32.load offset=212
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=16
    local.get 4
    local.get 5
    i32.store offset=20
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 13
    f32.store offset=424
    local.get 3
    local.get 3
    i32.load offset=424
    i32.store offset=200
    local.get 3
    local.get 9
    f32.store offset=420
    local.get 3
    local.get 8
    f32.store offset=416
    local.get 3
    i32.load offset=416
    local.set 5
    local.get 3
    i32.load offset=420
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=192
    local.get 3
    local.get 4
    i32.store offset=196
    local.get 2
    local.get 3
    i32.load offset=200
    i32.store offset=36
    local.get 3
    i32.load offset=192
    local.set 4
    local.get 3
    i32.load offset=196
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=28
    local.get 4
    local.get 5
    i32.store offset=32
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 10
    f32.store offset=408
    local.get 3
    local.get 3
    i32.load offset=408
    i32.store offset=184
    local.get 3
    local.get 9
    f32.store offset=404
    local.get 3
    local.get 12
    f32.store offset=400
    local.get 3
    i32.load offset=400
    local.set 5
    local.get 3
    i32.load offset=404
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=176
    local.get 3
    local.get 4
    i32.store offset=180
    local.get 2
    local.get 3
    i32.load offset=184
    i32.store offset=48
    local.get 3
    i32.load offset=176
    local.set 4
    local.get 3
    i32.load offset=180
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=40
    local.get 4
    local.get 5
    i32.store offset=44
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 13
    f32.store offset=392
    local.get 3
    local.get 3
    i32.load offset=392
    i32.store offset=168
    local.get 3
    local.get 9
    f32.store offset=388
    local.get 3
    local.get 12
    f32.store offset=384
    local.get 3
    i32.load offset=384
    local.set 5
    local.get 3
    i32.load offset=388
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=160
    local.get 3
    local.get 4
    i32.store offset=164
    local.get 2
    local.get 3
    i32.load offset=168
    i32.store offset=60
    local.get 3
    i32.load offset=160
    local.set 4
    local.get 3
    i32.load offset=164
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=52
    local.get 4
    local.get 5
    i32.store offset=56
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 10
    f32.store offset=376
    local.get 3
    local.get 3
    i32.load offset=376
    i32.store offset=152
    local.get 3
    local.get 11
    f32.store offset=372
    local.get 3
    local.get 8
    f32.store offset=368
    local.get 3
    i32.load offset=368
    local.set 5
    local.get 3
    i32.load offset=372
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=144
    local.get 3
    local.get 4
    i32.store offset=148
    local.get 2
    local.get 3
    i32.load offset=152
    i32.store offset=72
    local.get 3
    i32.load offset=144
    local.set 4
    local.get 3
    i32.load offset=148
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=64
    local.get 4
    local.get 5
    i32.store offset=68
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 13
    f32.store offset=360
    local.get 3
    local.get 3
    i32.load offset=360
    i32.store offset=136
    local.get 3
    local.get 11
    f32.store offset=356
    local.get 3
    local.get 8
    f32.store offset=352
    local.get 3
    i32.load offset=352
    local.set 5
    local.get 3
    i32.load offset=356
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=128
    local.get 3
    local.get 4
    i32.store offset=132
    local.get 2
    local.get 3
    i32.load offset=136
    i32.store offset=84
    local.get 3
    i32.load offset=128
    local.set 4
    local.get 3
    i32.load offset=132
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=76
    local.get 4
    local.get 5
    i32.store offset=80
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 10
    f32.store offset=344
    local.get 3
    local.get 3
    i32.load offset=344
    i32.store offset=120
    local.get 3
    local.get 11
    f32.store offset=340
    local.get 3
    local.get 12
    f32.store offset=336
    local.get 3
    i32.load offset=336
    local.set 5
    local.get 3
    i32.load offset=340
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=112
    local.get 3
    local.get 4
    i32.store offset=116
    local.get 2
    local.get 3
    i32.load offset=120
    i32.store offset=96
    local.get 3
    i32.load offset=112
    local.set 4
    local.get 3
    i32.load offset=116
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=88
    local.get 4
    local.get 5
    i32.store offset=92
    local.get 0
    i32.load offset=656
    local.set 2
    local.get 3
    local.get 13
    f32.store offset=328
    local.get 3
    local.get 3
    i32.load offset=328
    i32.store offset=104
    local.get 3
    local.get 11
    f32.store offset=324
    local.get 3
    local.get 12
    f32.store offset=320
    local.get 3
    i32.load offset=320
    local.set 5
    local.get 3
    i32.load offset=324
    local.set 4
    local.get 3
    local.get 5
    i32.store offset=96
    local.get 3
    local.get 4
    i32.store offset=100
    local.get 2
    local.get 3
    i32.load offset=104
    i32.store offset=108
    local.get 3
    i32.load offset=96
    local.set 4
    local.get 3
    i32.load offset=100
    local.set 5
    local.get 4
    local.set 6
    local.get 2
    local.set 4
    local.get 4
    local.get 6
    i32.store offset=100
    local.get 4
    local.get 5
    i32.store offset=104
    f32.const 0x1.fffffep+127 (;=3.40282e+38;)
    local.set 11
    f32.const 0x1.fffffep+127 (;=3.40282e+38;)
    local.set 9
    f32.const -0x1.fffffep+127 (;=-3.40282e+38;)
    local.set 12
    f32.const -0x1.fffffep+127 (;=-3.40282e+38;)
    local.set 10
    f32.const 0x1.fffffep+127 (;=3.40282e+38;)
    local.set 8
    i32.const 0
    local.set 2
    loop  ;; label = @1
      block  ;; label = @2
        local.get 0
        i32.load offset=656
        local.set 7
        local.get 2
        local.get 7
        i32.load offset=12
        i32.ge_s
        br_if 0 (;@2;)
        local.get 7
        local.get 2
        i32.const 12
        i32.mul
        i32.add
        local.set 6
        local.get 3
        local.get 6
        i32.load offset=24
        i32.store offset=456
        local.get 6
        i32.load offset=16
        local.set 5
        local.get 6
        i32.load offset=20
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=448
        local.get 3
        local.get 4
        i32.store offset=452
        local.get 3
        local.get 3
        i32.load offset=456
        i32.store offset=88
        local.get 3
        i32.load offset=448
        local.set 4
        local.get 3
        i32.load offset=452
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=80
        local.get 3
        local.get 5
        i32.store offset=84
        local.get 3
        i32.const 304
        i32.add
        local.get 1
        local.get 3
        i32.const 80
        i32.add
        i32.const 0
        call $j2625 ;; 
        local.get 3
        i32.load offset=312
        local.set 6
        local.get 3
        local.get 6
        i32.store offset=296
        local.get 3
        local.get 6
        i32.store offset=72
        local.get 3
        i32.load offset=304
        local.set 5
        local.get 3
        i32.load offset=308
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=288
        local.get 3
        local.get 4
        i32.store offset=292
        local.get 3
        local.get 5
        i32.store offset=64
        local.get 3
        local.get 4
        i32.store offset=68
        local.get 7
        local.get 2
        i32.const 12
        i32.mul
        i32.add
        local.set 7
        local.get 3
        i32.const -64
        i32.sub
        local.set 6
        local.get 7
        local.get 6
        i32.load offset=8
        i32.store offset=24
        local.get 6
        i32.load
        local.set 4
        local.get 6
        i32.load offset=4
        local.set 5
        local.get 4
        local.set 6
        local.get 7
        local.set 4
        local.get 4
        local.get 6
        i32.store offset=16
        local.get 4
        local.get 5
        i32.store offset=20
        local.get 11
        local.get 0
        i32.load offset=656
        local.get 2
        i32.const 12
        i32.mul
        i32.add
        f32.load offset=16
        f32.gt
        if  ;; label = @3
          local.get 0
          i32.load offset=656
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          f32.load offset=16
          local.set 11
        end
        local.get 9
        local.get 0
        i32.load offset=656
        local.get 2
        i32.const 12
        i32.mul
        i32.add
        f32.load offset=20
        f32.gt
        if  ;; label = @3
          local.get 0
          i32.load offset=656
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          f32.load offset=20
          local.set 9
        end
        local.get 12
        local.get 0
        i32.load offset=656
        local.get 2
        i32.const 12
        i32.mul
        i32.add
        f32.load offset=16
        f32.lt
        if  ;; label = @3
          local.get 0
          i32.load offset=656
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          f32.load offset=16
          local.set 12
        end
        local.get 10
        local.get 0
        i32.load offset=656
        local.get 2
        i32.const 12
        i32.mul
        i32.add
        f32.load offset=20
        f32.lt
        if  ;; label = @3
          local.get 0
          i32.load offset=656
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          f32.load offset=20
          local.set 10
        end
        block  ;; label = @3
          local.get 0
          i32.load offset=656
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          f32.load offset=24
          local.get 8
          f32.lt
          i32.eqz
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=656
          local.get 2
          i32.const 12
          i32.mul
          i32.add
          f32.load offset=24
          local.set 8
          local.get 8
          local.get 1
          i32.const 0
          call $j5331 ;; 
          f32.lt
          i32.eqz
          br_if 0 (;@3;)
          local.get 1
          i32.const 0
          call $j14061 ;; 
          f32.convert_i32_s
          local.set 12
          local.get 1
          i32.const 0
          call $j14063 ;; 
          f32.convert_i32_s
          local.set 10
          f32.const 0x0p+0 (;=0;)
          local.set 9
          f32.const 0x0p+0 (;=0;)
          local.set 11
          br 1 (;@2;)
        end
        local.get 2
        i32.const 1
        i32.add
        local.set 2
        br 1 (;@1;)
      end
    end
    block  ;; label = @1
      local.get 9
      local.get 10
      f32.eq
      if  ;; label = @2
        i32.const 0
        local.set 2
        br 1 (;@1;)
      end
      local.get 1
      i32.const 0
      call $j5331 ;; 
      local.get 8
      f32.gt
      if  ;; label = @2
        local.get 1
        i32.const 0
        call $j5331 ;; 
        f32.const 0x1.47ae14p-7 (;=0.01;)
        f32.add
        local.set 8
      end
      local.get 0
      f32.load offset=116
      f32.const 0x0p+0 (;=0;)
      f32.gt
      if  ;; label = @2
        local.get 0
        i32.load offset=144
        local.set 2
        local.get 0
        f32.load offset=136
        local.set 13
        local.get 3
        local.get 8
        f32.store offset=280
        local.get 3
        local.get 8
        f32.store offset=264
        local.get 3
        local.get 9
        f32.store offset=276
        local.get 3
        local.get 11
        f32.store offset=272
        local.get 3
        local.get 10
        f32.store offset=260
        local.get 3
        local.get 12
        f32.store offset=256
        local.get 13
        f32.const 0x1.4p+4 (;=20;)
        f32.mul
        f32.const 0x1.4p+3 (;=10;)
        f32.add
        local.get 2
        i32.const 5
        i32.mul
        f32.convert_i32_s
        f32.add
        f64.promote_f32
        local.set 14
        block  ;; label = @3
          local.get 14
          f64.abs
          f64.const 0x1p+31 (;=2.14748e+09;)
          f64.lt
          if  ;; label = @4
            local.get 14
            call $asm2wasm_f64_to_int
            local.set 2
            br 1 (;@3;)
          end
          i32.const -2147483648
          local.set 2
        end
        local.get 3
        local.get 3
        i32.load offset=280
        i32.store offset=56
        local.get 3
        local.get 3
        i32.load offset=264
        i32.store offset=40
        local.get 3
        i32.load offset=272
        local.set 5
        local.get 3
        i32.load offset=276
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=48
        local.get 3
        local.get 4
        i32.store offset=52
        local.get 3
        i32.load offset=256
        local.set 4
        local.get 3
        i32.load offset=260
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=32
        local.get 3
        local.get 5
        i32.store offset=36
        local.get 3
        local.get 1
        local.get 3
        i32.const 48
        i32.add
        local.get 3
        i32.const 32
        i32.add
        local.get 2
        local.get 0
        i32.const 592
        i32.add
        local.get 3
        call $j20235 ;; HighlightEffect.BuildMatrix
      end
      i32.const 1
      local.set 2
      local.get 0
      f32.load offset=156
      f32.const 0x0p+0 (;=0;)
      f32.gt
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=168
      local.set 7
      local.get 0
      f32.load offset=160
      local.set 13
      local.get 3
      local.get 8
      f32.store offset=248
      local.get 3
      local.get 8
      f32.store offset=232
      local.get 3
      local.get 9
      f32.store offset=244
      local.get 3
      local.get 11
      f32.store offset=240
      local.get 3
      local.get 10
      f32.store offset=228
      local.get 3
      local.get 12
      f32.store offset=224
      local.get 13
      f32.const 0x1.ep+4 (;=30;)
      f32.mul
      f32.const 0x1.4p+4 (;=20;)
      f32.add
      local.get 7
      i32.const 10
      i32.mul
      f32.convert_i32_s
      f32.add
      f64.promote_f32
      local.set 14
      block  ;; label = @2
        local.get 14
        f64.abs
        f64.const 0x1p+31 (;=2.14748e+09;)
        f64.lt
        if  ;; label = @3
          local.get 14
          call $asm2wasm_f64_to_int
          local.set 7
          br 1 (;@2;)
        end
        i32.const -2147483648
        local.set 7
      end
      local.get 3
      local.get 3
      i32.load offset=248
      i32.store offset=24
      local.get 3
      local.get 3
      i32.load offset=232
      i32.store offset=8
      local.get 3
      i32.load offset=240
      local.set 5
      local.get 3
      i32.load offset=244
      local.set 4
      local.get 3
      local.get 5
      i32.store offset=16
      local.get 3
      local.get 4
      i32.store offset=20
      local.get 3
      i32.load offset=224
      local.set 4
      local.get 3
      i32.load offset=228
      local.set 5
      local.get 3
      local.get 4
      i32.store
      local.get 3
      local.get 5
      i32.store offset=4
      local.get 3
      local.get 1
      local.get 3
      i32.const 16
      i32.add
      local.get 3
      local.get 7
      local.get 0
      i32.const 528
      i32.add
      local.get 3
      call $j20235 ;; HighlightEffect.BuildMatrix
    end
    local.get 3
    i32.const 464
    i32.add
    global.set 0
    local.get 2
    return)
