  (func $j39431 (type 6) (param i32 i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32 i32 i32 f32 f32 f32 f64)
    global.get 0
    i32.const 560
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4505036
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3427640
      i32.const 870
      call_indirect (type 0)
      i32.const 3550272
      i32.const 870
      call_indirect (type 0)
      i32.const 3550264
      i32.const 870
      call_indirect (type 0)
      i32.const 4505036
      i32.const 1
      i32.store8
    end
    local.get 3
    i32.const 504
    i32.add
    i32.const 0
    i32.const 52
    i32.const 890
    call_indirect (type 5)
    drop
    local.get 3
    i32.const 0
    i32.store offset=500
    local.get 0
    i32.const 512
    i32.add
    local.set 12
    local.get 0
    i32.load offset=512
    local.set 6
    block  ;; label = @1
      local.get 6
      if  ;; label = @2
        local.get 6
        i32.load offset=12
        i32.const 4
        i32.eq
        br_if 1 (;@1;)
      end
      local.get 12
      i32.const 3427640
      i32.load
      i32.const 4
      i32.const 888
      call_indirect (type 2)
      i32.store
      i32.const 0
      local.set 6
      loop  ;; label = @2
        block  ;; label = @3
          local.get 3
          local.get 6
          i32.store offset=500
          local.get 6
          i32.const 4
          i32.ge_s
          br_if 0 (;@3;)
          local.get 12
          i32.load
          local.set 9
          local.get 3
          i32.const 500
          i32.add
          i32.const 0
          i32.const 973
          call_indirect (type 2)
          local.set 5
          local.get 9
          local.get 6
          i32.const 2
          i32.shl
          i32.add
          i32.const 3550272
          i32.load
          local.get 5
          i32.const 0
          i32.const 910
          call_indirect (type 5)
          i32.const 0
          i32.const 1048
          call_indirect (type 2)
          i32.store offset=16
          local.get 3
          i32.load offset=500
          i32.const 1
          i32.add
          local.set 6
          br 1 (;@2;)
        end
      end
      i32.const 3550264
      i32.load
      i32.const 0
      i32.const 1048
      call_indirect (type 2)
      local.set 6
      local.get 0
      local.get 6
      i32.store offset=520
      local.get 0
      i32.load offset=512
      local.get 6
      i32.store offset=24
    end
    local.get 3
    i32.const 504
    i32.add
    local.get 0
    i32.const 660
    i32.add
    i32.const 52
    i32.const 883
    call_indirect (type 5)
    drop
    local.get 3
    i32.const 504
    i32.add
    i32.const 0
    i32.const 0
    i32.const 7859
    call_indirect (type 3)
    local.get 2
    f32.convert_i32_s
    local.set 14
    local.get 1
    f32.convert_i32_s
    local.set 15
    i32.const 0
    local.set 6
    loop  ;; label = @1
      local.get 6
      i32.const 4
      i32.lt_s
      if  ;; label = @2
        local.get 6
        i32.const 2
        call $i32s_div
        i32.const 2
        i32.add
        f32.convert_i32_s
        local.set 13
        local.get 15
        local.get 13
        f32.div
        f64.promote_f32
        local.set 16
        block  ;; label = @3
          local.get 16
          f64.abs
          f64.const 0x1p+31 (;=2.14748e+09;)
          f64.lt
          if  ;; label = @4
            local.get 16
            call $asm2wasm_f64_to_int
            local.set 1
            br 1 (;@3;)
          end
          i32.const -2147483648
          local.set 1
        end
        local.get 14
        local.get 13
        f32.div
        f64.promote_f32
        local.set 16
        block  ;; label = @3
          local.get 16
          f64.abs
          f64.const 0x1p+31 (;=2.14748e+09;)
          f64.lt
          if  ;; label = @4
            local.get 16
            call $asm2wasm_f64_to_int
            local.set 2
            br 1 (;@3;)
          end
          i32.const -2147483648
          local.set 2
        end
        local.get 3
        local.get 1
        i32.const 1
        local.get 1
        i32.const 1
        i32.gt_s
        select
        i32.store offset=504
        local.get 3
        local.get 2
        i32.const 1
        local.get 2
        i32.const 1
        i32.gt_s
        select
        i32.store offset=508
        local.get 0
        i32.load offset=504
        local.set 1
        local.get 0
        i32.load offset=512
        local.get 6
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.set 2
        local.get 3
        i32.const 4
        i32.add
        local.get 3
        i32.const 504
        i32.add
        i32.const 52
        i32.const 883
        call_indirect (type 5)
        drop
        local.get 1
        local.get 2
        local.get 3
        i32.const 4
        i32.add
        i32.const 1
        i32.const 0
        i32.const 13988
        call_indirect (type 7)
        local.get 6
        i32.const 1
        i32.add
        local.set 6
        br 1 (;@1;)
      end
    end
    local.get 0
    local.get 3
    call $j13406 ;; HighlightEffect.get_fxMatBlurOutline
    local.set 1
    i32.const 0
    local.set 6
    loop  ;; label = @1
      local.get 6
      i32.const 3
      i32.lt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=504
        local.set 2
        block  ;; label = @3
          local.get 6
          i32.eqz
          if  ;; label = @4
            local.get 3
            i32.const 440
            i32.add
            local.get 0
            i32.load offset=524
            i32.const 0
            i32.const 2787
            call_indirect (type 3)
            local.get 3
            local.get 3
            i32.load offset=464
            i32.store offset=496
            local.get 3
            i32.load offset=456
            local.set 5
            local.get 3
            i32.load offset=460
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=488
            local.get 3
            local.get 4
            i32.store offset=492
            local.get 3
            i32.load offset=448
            local.set 4
            local.get 3
            i32.load offset=452
            local.set 5
            local.get 3
            local.get 4
            i32.store offset=480
            local.get 3
            local.get 5
            i32.store offset=484
            local.get 3
            i32.load offset=440
            local.set 5
            local.get 3
            i32.load offset=444
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=472
            local.get 3
            local.get 4
            i32.store offset=476
            local.get 3
            i32.const 440
            i32.add
            local.get 0
            i32.load offset=512
            i32.load offset=20
            i32.const 0
            i32.const 2787
            call_indirect (type 3)
            local.get 3
            i32.load offset=464
            local.set 9
            local.get 3
            local.get 9
            i32.store offset=432
            local.get 3
            i32.load offset=456
            local.set 4
            local.get 3
            i32.load offset=460
            local.set 5
            local.get 4
            local.set 7
            local.get 5
            local.set 8
            local.get 3
            local.get 7
            i32.store offset=424
            local.get 3
            local.get 5
            i32.store offset=428
            local.get 3
            i32.load offset=448
            local.set 5
            local.get 3
            i32.load offset=452
            local.set 4
            local.get 5
            local.set 10
            local.get 4
            local.set 11
            local.get 3
            local.get 10
            i32.store offset=416
            local.get 3
            local.get 4
            i32.store offset=420
            local.get 3
            i32.load offset=480
            local.set 4
            local.get 3
            i32.load offset=484
            local.set 5
            local.get 3
            local.get 4
            i32.store offset=224
            local.get 3
            local.get 5
            i32.store offset=228
            local.get 3
            i32.load offset=488
            local.set 5
            local.get 3
            i32.load offset=492
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=232
            local.get 3
            local.get 4
            i32.store offset=236
            local.get 3
            local.get 3
            i32.load offset=496
            i32.store offset=240
            local.get 11
            local.set 4
            local.get 3
            local.get 10
            i32.store offset=192
            local.get 3
            local.get 4
            i32.store offset=196
            local.get 8
            local.set 4
            local.get 3
            local.get 7
            i32.store offset=200
            local.get 3
            local.get 4
            i32.store offset=204
            local.get 3
            local.get 9
            i32.store offset=208
            local.get 3
            i32.load offset=440
            local.set 4
            local.get 3
            i32.load offset=444
            local.set 5
            local.get 4
            local.set 7
            local.get 5
            local.set 8
            local.get 3
            local.get 7
            i32.store offset=408
            local.get 3
            local.get 5
            i32.store offset=412
            local.get 3
            i32.load offset=472
            local.set 5
            local.get 3
            i32.load offset=476
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=216
            local.get 3
            local.get 4
            i32.store offset=220
            local.get 8
            local.set 4
            local.get 3
            local.get 7
            i32.store offset=184
            local.get 3
            local.get 4
            i32.store offset=188
            local.get 2
            local.get 3
            i32.const 216
            i32.add
            local.get 3
            i32.const 184
            i32.add
            local.get 1
            i32.const 0
            i32.const 0
            i32.const 4360
            call_indirect (type 12)
            br 1 (;@3;)
          end
          local.get 3
          i32.const 440
          i32.add
          local.get 12
          i32.load
          local.get 6
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          i32.const 0
          i32.const 2787
          call_indirect (type 3)
          local.get 3
          local.get 3
          i32.load offset=464
          i32.store offset=496
          local.get 3
          i32.load offset=456
          local.set 4
          local.get 3
          i32.load offset=460
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=488
          local.get 3
          local.get 5
          i32.store offset=492
          local.get 3
          i32.load offset=448
          local.set 5
          local.get 3
          i32.load offset=452
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=480
          local.get 3
          local.get 4
          i32.store offset=484
          local.get 3
          i32.load offset=440
          local.set 4
          local.get 3
          i32.load offset=444
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=472
          local.get 3
          local.get 5
          i32.store offset=476
          local.get 3
          i32.const 440
          i32.add
          local.get 12
          i32.load
          local.get 6
          i32.const 1
          i32.add
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          i32.const 0
          i32.const 2787
          call_indirect (type 3)
          local.get 3
          i32.load offset=464
          local.set 9
          local.get 3
          local.get 9
          i32.store offset=400
          local.get 3
          i32.load offset=456
          local.set 5
          local.get 3
          i32.load offset=460
          local.set 4
          local.get 5
          local.set 7
          local.get 4
          local.set 8
          local.get 3
          local.get 7
          i32.store offset=392
          local.get 3
          local.get 4
          i32.store offset=396
          local.get 3
          i32.load offset=448
          local.set 4
          local.get 3
          i32.load offset=452
          local.set 5
          local.get 4
          local.set 10
          local.get 5
          local.set 11
          local.get 3
          local.get 10
          i32.store offset=384
          local.get 3
          local.get 5
          i32.store offset=388
          local.get 3
          i32.load offset=480
          local.set 5
          local.get 3
          i32.load offset=484
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=288
          local.get 3
          local.get 4
          i32.store offset=292
          local.get 3
          i32.load offset=488
          local.set 4
          local.get 3
          i32.load offset=492
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=296
          local.get 3
          local.get 5
          i32.store offset=300
          local.get 3
          local.get 3
          i32.load offset=496
          i32.store offset=304
          local.get 11
          local.set 5
          local.get 3
          local.get 10
          i32.store offset=256
          local.get 3
          local.get 5
          i32.store offset=260
          local.get 8
          local.set 5
          local.get 3
          local.get 7
          i32.store offset=264
          local.get 3
          local.get 5
          i32.store offset=268
          local.get 3
          local.get 9
          i32.store offset=272
          local.get 3
          i32.load offset=440
          local.set 5
          local.get 3
          i32.load offset=444
          local.set 4
          local.get 5
          local.set 7
          local.get 4
          local.set 8
          local.get 3
          local.get 7
          i32.store offset=376
          local.get 3
          local.get 4
          i32.store offset=380
          local.get 3
          i32.load offset=472
          local.set 4
          local.get 3
          i32.load offset=476
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=280
          local.get 3
          local.get 5
          i32.store offset=284
          local.get 8
          local.set 5
          local.get 3
          local.get 7
          i32.store offset=248
          local.get 3
          local.get 5
          i32.store offset=252
          local.get 2
          local.get 3
          i32.const 280
          i32.add
          local.get 3
          i32.const 248
          i32.add
          local.get 1
          i32.const 0
          i32.const 0
          i32.const 4360
          call_indirect (type 12)
        end
        local.get 0
        i32.load offset=504
        local.set 2
        local.get 3
        i32.const 440
        i32.add
        local.get 0
        i32.load offset=512
        local.get 6
        i32.const 1
        i32.add
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        i32.const 0
        i32.const 2787
        call_indirect (type 3)
        local.get 3
        local.get 3
        i32.load offset=464
        i32.store offset=496
        local.get 3
        i32.load offset=456
        local.set 5
        local.get 3
        i32.load offset=460
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=488
        local.get 3
        local.get 4
        i32.store offset=492
        local.get 3
        i32.load offset=448
        local.set 4
        local.get 3
        i32.load offset=452
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=480
        local.get 3
        local.get 5
        i32.store offset=484
        local.get 3
        i32.load offset=440
        local.set 5
        local.get 3
        i32.load offset=444
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=472
        local.get 3
        local.get 4
        i32.store offset=476
        local.get 3
        i32.const 440
        i32.add
        local.get 0
        i32.load offset=512
        local.get 6
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        i32.const 0
        i32.const 2787
        call_indirect (type 3)
        local.get 3
        i32.load offset=464
        local.set 9
        local.get 3
        local.get 9
        i32.store offset=368
        local.get 3
        i32.load offset=456
        local.set 4
        local.get 3
        i32.load offset=460
        local.set 5
        local.get 4
        local.set 7
        local.get 5
        local.set 8
        local.get 3
        local.get 7
        i32.store offset=360
        local.get 3
        local.get 5
        i32.store offset=364
        local.get 3
        i32.load offset=448
        local.set 5
        local.get 3
        i32.load offset=452
        local.set 4
        local.get 5
        local.set 10
        local.get 4
        local.set 11
        local.get 3
        local.get 10
        i32.store offset=352
        local.get 3
        local.get 4
        i32.store offset=356
        local.get 3
        i32.load offset=480
        local.set 4
        local.get 3
        i32.load offset=484
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=160
        local.get 3
        local.get 5
        i32.store offset=164
        local.get 3
        i32.load offset=488
        local.set 5
        local.get 3
        i32.load offset=492
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=168
        local.get 3
        local.get 4
        i32.store offset=172
        local.get 3
        local.get 3
        i32.load offset=496
        i32.store offset=176
        local.get 11
        local.set 4
        local.get 3
        local.get 10
        i32.store offset=128
        local.get 3
        local.get 4
        i32.store offset=132
        local.get 8
        local.set 4
        local.get 3
        local.get 7
        i32.store offset=136
        local.get 3
        local.get 4
        i32.store offset=140
        local.get 3
        local.get 9
        i32.store offset=144
        local.get 3
        i32.load offset=440
        local.set 4
        local.get 3
        i32.load offset=444
        local.set 5
        local.get 4
        local.set 7
        local.get 5
        local.set 8
        local.get 3
        local.get 7
        i32.store offset=344
        local.get 3
        local.get 5
        i32.store offset=348
        local.get 3
        i32.load offset=472
        local.set 5
        local.get 3
        i32.load offset=476
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=152
        local.get 3
        local.get 4
        i32.store offset=156
        local.get 8
        local.set 4
        local.get 3
        local.get 7
        i32.store offset=120
        local.get 3
        local.get 4
        i32.store offset=124
        local.get 2
        local.get 3
        i32.const 152
        i32.add
        local.get 3
        i32.const 120
        i32.add
        local.get 1
        i32.const 1
        i32.const 0
        i32.const 4360
        call_indirect (type 12)
        local.get 6
        i32.const 2
        i32.lt_s
        if  ;; label = @3
          local.get 0
          i32.load offset=504
          local.set 2
          local.get 3
          i32.const 440
          i32.add
          local.get 0
          i32.load offset=512
          local.get 6
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          i32.const 0
          i32.const 2787
          call_indirect (type 3)
          local.get 3
          local.get 3
          i32.load offset=464
          i32.store offset=496
          local.get 3
          i32.load offset=456
          local.set 4
          local.get 3
          i32.load offset=460
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=488
          local.get 3
          local.get 5
          i32.store offset=492
          local.get 3
          i32.load offset=448
          local.set 5
          local.get 3
          i32.load offset=452
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=480
          local.get 3
          local.get 4
          i32.store offset=484
          local.get 3
          i32.load offset=440
          local.set 4
          local.get 3
          i32.load offset=444
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=472
          local.get 3
          local.get 5
          i32.store offset=476
          local.get 3
          i32.const 440
          i32.add
          local.get 0
          i32.load offset=512
          local.get 6
          i32.const 2
          i32.add
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          i32.const 0
          i32.const 2787
          call_indirect (type 3)
          local.get 3
          i32.load offset=464
          local.set 9
          local.get 3
          local.get 9
          i32.store offset=336
          local.get 3
          i32.load offset=456
          local.set 5
          local.get 3
          i32.load offset=460
          local.set 4
          local.get 5
          local.set 7
          local.get 4
          local.set 8
          local.get 3
          local.get 7
          i32.store offset=328
          local.get 3
          local.get 4
          i32.store offset=332
          local.get 3
          i32.load offset=448
          local.set 4
          local.get 3
          i32.load offset=452
          local.set 5
          local.get 4
          local.set 10
          local.get 5
          local.set 11
          local.get 3
          local.get 10
          i32.store offset=320
          local.get 3
          local.get 5
          i32.store offset=324
          local.get 3
          i32.load offset=480
          local.set 5
          local.get 3
          i32.load offset=484
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=96
          local.get 3
          local.get 4
          i32.store offset=100
          local.get 3
          i32.load offset=488
          local.set 4
          local.get 3
          i32.load offset=492
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=104
          local.get 3
          local.get 5
          i32.store offset=108
          local.get 3
          local.get 3
          i32.load offset=496
          i32.store offset=112
          local.get 11
          local.set 5
          local.get 3
          i32.const -64
          i32.sub
          local.set 4
          local.get 4
          local.get 10
          i32.store
          local.get 4
          local.get 5
          i32.store offset=4
          local.get 8
          local.set 5
          local.get 3
          local.get 7
          i32.store offset=72
          local.get 3
          local.get 5
          i32.store offset=76
          local.get 3
          local.get 9
          i32.store offset=80
          local.get 3
          i32.load offset=440
          local.set 5
          local.get 3
          i32.load offset=444
          local.set 4
          local.get 5
          local.set 7
          local.get 4
          local.set 8
          local.get 3
          local.get 7
          i32.store offset=312
          local.get 3
          local.get 4
          i32.store offset=316
          local.get 3
          i32.load offset=472
          local.set 4
          local.get 3
          i32.load offset=476
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=88
          local.get 3
          local.get 5
          i32.store offset=92
          local.get 8
          local.set 5
          local.get 3
          local.get 7
          i32.store offset=56
          local.get 3
          local.get 5
          i32.store offset=60
          local.get 2
          local.get 3
          i32.const 88
          i32.add
          local.get 3
          i32.const 56
          i32.add
          local.get 1
          i32.const 2
          i32.const 0
          i32.const 4360
          call_indirect (type 12)
        end
        local.get 6
        i32.const 2
        i32.add
        local.set 6
        br 1 (;@1;)
      end
    end
    local.get 3
    i32.const 560
    i32.add
    global.set 0)
