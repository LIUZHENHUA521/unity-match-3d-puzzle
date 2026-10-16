  (func $j39446 (type 7) (param i32 i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32 i32 f32 f32)
    global.get 0
    i32.const 640
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4505037
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432324
      call $j870 ;; 
      i32.const 3432484
      call $j870 ;; 
      i32.const 3437276
      call $j870 ;; 
      i32.const 4505037
      i32.const 1
      i32.store8
    end
    local.get 3
    i32.const 0
    i32.store offset=632
    i32.const 0
    local.set 4
    local.get 3
    i32.const 0
    i32.store offset=624
    local.get 3
    local.get 4
    i32.store offset=628
    i32.const 0
    local.set 4
    local.get 3
    i32.const 0
    i32.store offset=616
    local.get 3
    local.get 4
    i32.store offset=620
    i32.const 0
    local.set 4
    local.get 3
    i32.const 0
    i32.store offset=608
    local.get 3
    local.get 4
    i32.store offset=612
    i32.const 0
    local.set 4
    local.get 3
    i32.const 0
    i32.store offset=600
    local.get 3
    local.get 4
    i32.store offset=604
    i32.const 0
    local.set 4
    local.get 3
    i32.const 0
    i32.store offset=592
    local.get 3
    local.get 4
    i32.store offset=596
    i32.const 0
    local.set 4
    local.get 3
    i32.const 0
    i32.store offset=584
    local.get 3
    local.get 4
    i32.store offset=588
    i32.const 0
    local.set 4
    local.get 3
    i32.const 0
    i32.store offset=576
    local.get 3
    local.get 4
    i32.store offset=580
    block  ;; label = @1
      local.get 0
      f32.load offset=156
      f32.const 0x0p+0 (;=0;)
      f32.gt
      if  ;; label = @2
        local.get 0
        i32.load offset=164
        local.set 9
        local.get 3
        i32.const 608
        i32.add
        i32.const 2
        i32.const 0
        i32.const -1
        i32.const -1
        i32.const 0
        call $j9569 ;; 
        i32.const 1
        local.set 7
        local.get 9
        i32.const 2
        i32.ne
        br_if 1 (;@1;)
        local.get 0
        local.get 0
        call $j8123 ;; HighlightEffect.get_fxMatComposeGlow
        local.set 9
        i32.const 3437276
        i32.load
        call $j871 ;; 
        i32.const 3437276
        i32.load
        i32.load offset=92
        i32.load offset=24
        local.set 6
        i32.const 0
        call $j8878 ;; 
        if  ;; label = @3
          local.get 0
          i32.load8_u offset=68
          local.set 7
          f32.const 0x1p+0 (;=1;)
          f32.const 0x0p+0 (;=0;)
          local.get 7
          select
          local.set 13
          f32.const -0x1p+0 (;=-1;)
          f32.const 0x1p+0 (;=1;)
          local.get 7
          select
          local.set 12
        else
          f32.const 0x1p+0 (;=1;)
          local.set 12
        end
        i32.const 0
        local.set 4
        local.get 3
        i32.const 0
        i32.store offset=568
        local.get 3
        local.get 4
        i32.store offset=572
        i32.const 0
        local.set 4
        local.get 3
        i32.const 0
        i32.store offset=440
        local.get 3
        local.get 4
        i32.store offset=444
        local.get 3
        local.get 12
        f32.store offset=564
        local.get 3
        local.get 13
        f32.store offset=560
        local.get 3
        i32.load offset=560
        local.set 4
        local.get 3
        i32.load offset=564
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=432
        local.get 3
        local.get 5
        i32.store offset=436
        i32.const 0
        local.set 7
        local.get 9
        local.get 6
        local.get 3
        i32.const 432
        i32.add
        i32.const 0
        call $j2543 ;; 
        local.get 0
        i32.load8_u offset=208
        if  ;; label = @3
          i32.const 0
          call $j3100 ;; 
          local.set 6
          local.get 0
          i32.load offset=504
          local.set 7
          block  ;; label = @4
            block  ;; label = @5
              local.get 6
              i32.eqz
              br_if 0 (;@5;)
              local.get 1
              i32.const 1
              i32.eq
              br_if 0 (;@5;)
              local.get 3
              local.get 3
              i32.load offset=632
              i32.store offset=392
              local.get 3
              i32.load offset=624
              local.set 5
              local.get 3
              i32.load offset=628
              local.set 4
              local.get 3
              local.get 5
              i32.store offset=384
              local.get 3
              local.get 4
              i32.store offset=388
              local.get 3
              i32.load offset=616
              local.set 4
              local.get 3
              i32.load offset=620
              local.set 5
              local.get 3
              local.get 4
              i32.store offset=376
              local.get 3
              local.get 5
              i32.store offset=380
              local.get 3
              i32.load offset=608
              local.set 5
              local.get 3
              i32.load offset=612
              local.set 4
              local.get 3
              local.get 5
              i32.store offset=368
              local.get 3
              local.get 4
              i32.store offset=372
              local.get 7
              local.get 3
              i32.const 368
              i32.add
              i32.const 0
              i32.const 3
              i32.const 0
              call $j13985 ;; 
              br 1 (;@4;)
            end
            local.get 3
            local.get 3
            i32.load offset=632
            i32.store offset=424
            local.get 3
            i32.load offset=624
            local.set 4
            local.get 3
            i32.load offset=628
            local.set 5
            local.get 3
            local.get 4
            i32.store offset=416
            local.get 3
            local.get 5
            i32.store offset=420
            local.get 3
            i32.load offset=616
            local.set 5
            local.get 3
            i32.load offset=620
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=408
            local.get 3
            local.get 4
            i32.store offset=412
            local.get 3
            i32.load offset=608
            local.set 4
            local.get 3
            i32.load offset=612
            local.set 5
            local.get 3
            local.get 4
            i32.store offset=400
            local.get 3
            local.get 5
            i32.store offset=404
            i32.const 41142
            local.get 7
            local.get 3
            i32.const 400
            i32.add
            i32.const 0
            call $redir_viii
          end
          i32.const 0
          local.set 7
          i32.const 3437276
          i32.load
          call $j871 ;; 
          local.get 9
          i32.const 3437276
          i32.load
          i32.load offset=92
          i32.load offset=20
          i32.const 8
          i32.const 5
          i32.const 4
          local.get 1
          i32.const 2
          i32.eq
          select
          local.get 1
          i32.const 1
          i32.eq
          select
          i32.const 0
          call $j1524 ;; 
          i32.const 3437276
          i32.load
          i32.load offset=92
          i32.load offset=28
          local.set 1
          local.get 0
          i32.const 712
          i32.add
          local.get 0
          i32.const 728
          i32.add
          local.get 0
          i32.load8_u offset=209
          select
          local.set 6
          local.get 6
          local.set 8
          local.get 8
          i32.load offset=8
          local.set 5
          local.get 8
          i32.load offset=12
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=600
          local.get 3
          local.get 4
          i32.store offset=604
          local.get 8
          i32.load
          local.set 4
          local.get 8
          i32.load offset=4
          local.set 5
          local.get 4
          local.set 10
          local.get 5
          local.set 8
          local.get 3
          i32.load offset=600
          local.set 5
          local.get 3
          i32.load offset=604
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=360
          local.get 3
          local.get 4
          i32.store offset=364
          local.get 8
          local.set 4
          local.get 3
          local.get 10
          i32.store offset=592
          local.get 3
          local.get 4
          i32.store offset=596
          local.get 3
          i32.load offset=592
          local.set 4
          local.get 3
          i32.load offset=596
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=352
          local.get 3
          local.get 5
          i32.store offset=356
          local.get 9
          local.get 1
          local.get 3
          i32.const 352
          i32.add
          i32.const 0
          call $j2932 ;; 
          local.get 0
          i32.load offset=504
          local.set 1
          i32.const 3432484
          i32.load
          call $j871 ;; 
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=64
          local.set 6
          local.get 3
          i32.const 288
          i32.add
          local.get 0
          i32.const 528
          i32.add
          i32.const 64
          call $j883 ;; 
          drop
          local.get 1
          local.get 6
          local.get 3
          i32.const 288
          i32.add
          local.get 9
          i32.const 0
          i32.const 0
          i32.const 0
          call $j6886 ;; 
          br 2 (;@1;)
        end
        local.get 0
        i32.load offset=504
        local.set 1
        local.get 3
        i32.const 528
        i32.add
        local.get 0
        i32.load offset=516
        i32.const 0
        call $j2787 ;; 
        local.get 3
        i32.load offset=552
        local.set 6
        local.get 3
        local.get 6
        i32.store offset=520
        local.get 3
        i32.load offset=544
        local.set 5
        local.get 3
        i32.load offset=548
        local.set 4
        local.get 5
        local.set 10
        local.get 4
        local.set 8
        local.get 3
        local.get 10
        i32.store offset=512
        local.get 3
        local.get 4
        i32.store offset=516
        local.get 3
        i32.load offset=536
        local.set 4
        local.get 3
        i32.load offset=540
        local.set 5
        local.get 4
        local.set 11
        local.get 3
        local.get 11
        i32.store offset=504
        local.get 3
        local.get 5
        i32.store offset=508
        local.get 3
        local.get 11
        i32.store offset=264
        local.get 3
        local.get 5
        i32.store offset=268
        local.get 8
        local.set 5
        local.get 3
        local.get 10
        i32.store offset=272
        local.get 3
        local.get 5
        i32.store offset=276
        local.get 3
        local.get 6
        i32.store offset=280
        local.get 3
        i32.load offset=616
        local.set 5
        local.get 3
        i32.load offset=620
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=232
        local.get 3
        local.get 4
        i32.store offset=236
        local.get 3
        i32.load offset=624
        local.set 4
        local.get 3
        i32.load offset=628
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=240
        local.get 3
        local.get 5
        i32.store offset=244
        local.get 3
        local.get 3
        i32.load offset=632
        i32.store offset=248
        local.get 3
        i32.load offset=528
        local.set 5
        local.get 3
        i32.load offset=532
        local.set 4
        local.get 5
        local.set 10
        local.get 3
        local.get 10
        i32.store offset=496
        local.get 3
        local.get 4
        i32.store offset=500
        local.get 3
        local.get 10
        i32.store offset=256
        local.get 3
        local.get 4
        i32.store offset=260
        local.get 3
        i32.load offset=608
        local.set 4
        local.get 3
        i32.load offset=612
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=224
        local.get 3
        local.get 5
        i32.store offset=228
        local.get 1
        local.get 3
        i32.const 256
        i32.add
        local.get 3
        i32.const 224
        i32.add
        local.get 9
        i32.const 1
        i32.const 0
        call $j4360 ;; 
        br 1 (;@1;)
      end
      local.get 3
      i32.const 608
      i32.add
      i32.const 2
      i32.const 0
      i32.const -1
      i32.const -1
      i32.const 0
      call $j9569 ;; 
      i32.const 1
      local.set 7
    end
    i32.const 1
    local.set 9
    block  ;; label = @1
      local.get 0
      f32.load offset=116
      f32.const 0x0p+0 (;=0;)
      f32.gt
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=140
      i32.const 2
      i32.ne
      br_if 0 (;@1;)
      local.get 0
      local.get 0
      call $j9988 ;; HighlightEffect.get_fxMatComposeOutline
      local.set 1
      i32.const 3437276
      i32.load
      call $j871 ;; 
      i32.const 3437276
      i32.load
      i32.load offset=92
      i32.load offset=24
      local.set 9
      block  ;; label = @2
        i32.const 0
        call $j8878 ;; 
        i32.eqz
        if  ;; label = @3
          f32.const 0x0p+0 (;=0;)
          local.set 13
          f32.const 0x1p+0 (;=1;)
          local.set 12
          br 1 (;@2;)
        end
        local.get 0
        i32.load8_u offset=68
        local.set 6
        f32.const 0x1p+0 (;=1;)
        f32.const 0x0p+0 (;=0;)
        local.get 6
        select
        local.set 13
        f32.const -0x1p+0 (;=-1;)
        f32.const 0x1p+0 (;=1;)
        local.get 6
        select
        local.set 12
      end
      i32.const 0
      local.set 5
      local.get 3
      i32.const 0
      i32.store offset=488
      local.get 3
      local.get 5
      i32.store offset=492
      i32.const 0
      local.set 5
      local.get 3
      i32.const 0
      i32.store offset=216
      local.get 3
      local.get 5
      i32.store offset=220
      local.get 3
      local.get 12
      f32.store offset=484
      local.get 3
      local.get 13
      f32.store offset=480
      local.get 3
      i32.load offset=480
      local.set 5
      local.get 3
      i32.load offset=484
      local.set 4
      local.get 3
      local.get 5
      i32.store offset=208
      local.get 3
      local.get 4
      i32.store offset=212
      local.get 1
      local.get 9
      local.get 3
      i32.const 208
      i32.add
      i32.const 0
      call $j2543 ;; 
      local.get 0
      i32.load8_u offset=152
      if  ;; label = @2
        i32.const 0
        call $j3100 ;; 
        local.set 6
        local.get 0
        i32.load offset=504
        local.set 9
        block  ;; label = @3
          block  ;; label = @4
            local.get 6
            i32.eqz
            br_if 0 (;@4;)
            local.get 2
            i32.const 1
            i32.eq
            br_if 0 (;@4;)
            local.get 3
            local.get 3
            i32.load offset=632
            i32.store offset=168
            local.get 3
            i32.load offset=624
            local.set 4
            local.get 3
            i32.load offset=628
            local.set 5
            local.get 3
            local.get 4
            i32.store offset=160
            local.get 3
            local.get 5
            i32.store offset=164
            local.get 3
            i32.load offset=616
            local.set 5
            local.get 3
            i32.load offset=620
            local.set 4
            local.get 3
            local.get 5
            i32.store offset=152
            local.get 3
            local.get 4
            i32.store offset=156
            local.get 3
            i32.load offset=608
            local.set 4
            local.get 3
            i32.load offset=612
            local.set 5
            local.get 3
            local.get 4
            i32.store offset=144
            local.get 3
            local.get 5
            i32.store offset=148
            local.get 9
            local.get 3
            i32.const 144
            i32.add
            i32.const 0
            i32.const 3
            i32.const 0
            call $j13985 ;; 
            br 1 (;@3;)
          end
          local.get 3
          local.get 3
          i32.load offset=632
          i32.store offset=200
          local.get 3
          i32.load offset=624
          local.set 5
          local.get 3
          i32.load offset=628
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=192
          local.get 3
          local.get 4
          i32.store offset=196
          local.get 3
          i32.load offset=616
          local.set 4
          local.get 3
          i32.load offset=620
          local.set 5
          local.get 3
          local.get 4
          i32.store offset=184
          local.get 3
          local.get 5
          i32.store offset=188
          local.get 3
          i32.load offset=608
          local.set 5
          local.get 3
          i32.load offset=612
          local.set 4
          local.get 3
          local.get 5
          i32.store offset=176
          local.get 3
          local.get 4
          i32.store offset=180
          i32.const 41142
          local.get 9
          local.get 3
          i32.const 176
          i32.add
          i32.const 0
          call $redir_viii
        end
        i32.const 0
        local.set 9
        i32.const 3437276
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 3437276
        i32.load
        i32.load offset=92
        i32.load offset=20
        i32.const 8
        i32.const 5
        i32.const 4
        local.get 2
        i32.const 2
        i32.eq
        select
        local.get 2
        i32.const 1
        i32.eq
        select
        i32.const 0
        call $j1524 ;; 
        i32.const 3437276
        i32.load
        i32.load offset=92
        i32.load offset=28
        local.set 2
        local.get 0
        i32.const 712
        i32.add
        local.get 0
        i32.const 728
        i32.add
        local.get 0
        i32.load8_u offset=153
        select
        local.set 6
        local.get 6
        local.set 8
        local.get 8
        i32.load offset=8
        local.set 4
        local.get 8
        i32.load offset=12
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=584
        local.get 3
        local.get 5
        i32.store offset=588
        local.get 8
        i32.load
        local.set 5
        local.get 8
        i32.load offset=4
        local.set 4
        local.get 5
        local.set 10
        local.get 4
        local.set 8
        local.get 3
        i32.load offset=584
        local.set 4
        local.get 3
        i32.load offset=588
        local.set 5
        local.get 3
        local.get 4
        i32.store offset=136
        local.get 3
        local.get 5
        i32.store offset=140
        local.get 8
        local.set 5
        local.get 3
        local.get 10
        i32.store offset=576
        local.get 3
        local.get 5
        i32.store offset=580
        local.get 3
        i32.load offset=576
        local.set 5
        local.get 3
        i32.load offset=580
        local.set 4
        local.get 3
        local.get 5
        i32.store offset=128
        local.get 3
        local.get 4
        i32.store offset=132
        local.get 1
        local.get 2
        local.get 3
        i32.const 128
        i32.add
        i32.const 0
        call $j2932 ;; 
        local.get 0
        i32.load offset=504
        local.set 2
        i32.const 3432484
        i32.load
        call $j871 ;; 
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=64
        local.set 6
        local.get 3
        i32.const -64
        i32.sub
        local.get 0
        i32.const 592
        i32.add
        i32.const 64
        call $j883 ;; 
        drop
        local.get 2
        local.get 6
        local.get 3
        i32.const -64
        i32.sub
        local.get 1
        i32.const 0
        i32.const 0
        i32.const 0
        call $j6886 ;; 
        br 1 (;@1;)
      end
      local.get 0
      i32.load offset=504
      local.set 2
      i32.const 0
      local.set 9
      local.get 3
      i32.const 528
      i32.add
      local.get 0
      i32.load offset=520
      i32.const 0
      call $j2787 ;; 
      local.get 3
      i32.load offset=552
      local.set 6
      local.get 3
      local.get 6
      i32.store offset=472
      local.get 3
      i32.load offset=544
      local.set 4
      local.get 3
      i32.load offset=548
      local.set 5
      local.get 4
      local.set 10
      local.get 5
      local.set 8
      local.get 3
      local.get 10
      i32.store offset=464
      local.get 3
      local.get 5
      i32.store offset=468
      local.get 3
      i32.load offset=536
      local.set 5
      local.get 3
      i32.load offset=540
      local.set 4
      local.get 5
      local.set 11
      local.get 3
      local.get 11
      i32.store offset=456
      local.get 3
      local.get 4
      i32.store offset=460
      local.get 3
      local.get 11
      i32.store offset=40
      local.get 3
      local.get 4
      i32.store offset=44
      local.get 8
      local.set 4
      local.get 3
      local.get 10
      i32.store offset=48
      local.get 3
      local.get 4
      i32.store offset=52
      local.get 3
      local.get 6
      i32.store offset=56
      local.get 3
      i32.load offset=616
      local.set 4
      local.get 3
      i32.load offset=620
      local.set 5
      local.get 3
      local.get 4
      i32.store offset=8
      local.get 3
      local.get 5
      i32.store offset=12
      local.get 3
      i32.load offset=624
      local.set 5
      local.get 3
      i32.load offset=628
      local.set 4
      local.get 3
      local.get 5
      i32.store offset=16
      local.get 3
      local.get 4
      i32.store offset=20
      local.get 3
      local.get 3
      i32.load offset=632
      i32.store offset=24
      local.get 3
      i32.load offset=528
      local.set 4
      local.get 3
      i32.load offset=532
      local.set 5
      local.get 4
      local.set 10
      local.get 3
      local.get 10
      i32.store offset=448
      local.get 3
      local.get 5
      i32.store offset=452
      local.get 3
      local.get 10
      i32.store offset=32
      local.get 3
      local.get 5
      i32.store offset=36
      local.get 3
      i32.load offset=608
      local.set 5
      local.get 3
      i32.load offset=612
      local.set 4
      local.get 3
      local.get 5
      i32.store
      local.get 3
      local.get 4
      i32.store offset=4
      local.get 2
      local.get 3
      i32.const 32
      i32.add
      local.get 3
      local.get 1
      i32.const 1
      i32.const 0
      call $j4360 ;; 
    end
    block  ;; label = @1
      local.get 7
      br_if 0 (;@1;)
      i32.const 0
      local.set 7
      loop  ;; label = @2
        local.get 0
        i32.load offset=508
        local.set 1
        local.get 7
        local.get 1
        i32.load offset=12
        i32.ge_s
        br_if 1 (;@1;)
        local.get 0
        i32.load offset=504
        local.get 1
        local.get 7
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        i32.const 0
        call $j13987 ;; 
        local.get 7
        i32.const 1
        i32.add
        local.set 7
        br 0 (;@2;)
      end
      unreachable
    end
    block  ;; label = @1
      local.get 9
      br_if 0 (;@1;)
      i32.const 0
      local.set 7
      loop  ;; label = @2
        local.get 0
        i32.load offset=512
        local.set 1
        local.get 7
        local.get 1
        i32.load offset=12
        i32.ge_s
        br_if 1 (;@1;)
        local.get 0
        i32.load offset=504
        local.get 1
        local.get 7
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        i32.const 0
        call $j13987 ;; 
        local.get 7
        i32.const 1
        i32.add
        local.set 7
        br 0 (;@2;)
      end
      unreachable
    end
    local.get 0
    i32.load offset=504
    local.get 0
    i32.load offset=524
    i32.const 0
    call $j13987 ;; 
    local.get 0
    i32.load offset=504
    local.set 0
    i32.const 3432324
    i32.load
    call $j871 ;; 
    local.get 0
    i32.const 0
    call $j3211 ;; 
    local.get 3
    i32.const 640
    i32.add
    global.set 0)
