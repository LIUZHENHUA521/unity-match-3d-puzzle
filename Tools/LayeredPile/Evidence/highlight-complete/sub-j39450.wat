  (func $j39450 (type 3) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 368
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4505064
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3485768
      i32.const 870
      call_indirect (type 0)
      i32.const 3451168
      i32.const 870
      call_indirect (type 0)
      i32.const 3451172
      i32.const 870
      call_indirect (type 0)
      i32.const 3432324
      i32.const 870
      call_indirect (type 0)
      i32.const 3432484
      i32.const 870
      call_indirect (type 0)
      i32.const 3470972
      i32.const 870
      call_indirect (type 0)
      i32.const 3470980
      i32.const 870
      call_indirect (type 0)
      i32.const 3470984
      i32.const 870
      call_indirect (type 0)
      i32.const 3470968
      i32.const 870
      call_indirect (type 0)
      i32.const 3470992
      i32.const 870
      call_indirect (type 0)
      i32.const 3470996
      i32.const 870
      call_indirect (type 0)
      i32.const 3424976
      i32.const 870
      call_indirect (type 0)
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 3427900
      i32.const 870
      call_indirect (type 0)
      i32.const 4505064
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 0
    i32.store offset=364
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=352
    local.get 2
    local.get 3
    i32.store offset=356
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=344
    local.get 2
    local.get 3
    i32.store offset=348
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=336
    local.get 2
    local.get 3
    i32.store offset=340
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=328
    local.get 2
    local.get 3
    i32.store offset=332
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=320
    local.get 2
    local.get 3
    i32.store offset=324
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=312
    local.get 2
    local.get 3
    i32.store offset=316
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=304
    local.get 2
    local.get 3
    i32.store offset=308
    i32.const 0
    local.set 3
    local.get 2
    i32.const 0
    i32.store offset=296
    local.get 2
    local.get 3
    i32.store offset=300
    local.get 0
    i32.load offset=948
    local.get 1
    local.get 2
    i32.const 364
    i32.add
    i32.const 3451168
    i32.load
    i32.const 1012
    call_indirect (type 8)
    i32.eqz
    if  ;; label = @1
      i32.const 3424976
      i32.load
      i32.const 873
      call_indirect (type 1)
      local.set 5
      local.get 5
      i32.const 3470968
      i32.load
      i32.const 918
      call_indirect (type 4)
      local.get 2
      local.get 5
      i32.store offset=364
      local.get 0
      i32.load offset=948
      local.get 1
      local.get 5
      i32.const 3451172
      i32.load
      i32.const 1016
      call_indirect (type 6)
    end
    i32.const 0
    i32.const 1519
    call_indirect (type 21)
    local.set 8
    i32.const 0
    i32.const 2109
    call_indirect (type 1)
    local.set 5
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          i32.const 0
          i32.const 1519
          call_indirect (type 21)
          local.get 0
          f32.load offset=752
          f32.sub
          local.get 0
          f32.load offset=344
          f32.lt
          i32.eqz
          br_if 0 (;@3;)
          i32.const 0
          i32.const 1337
          call_indirect (type 1)
          i32.eqz
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=756
          local.get 5
          i32.ne
          br_if 1 (;@2;)
        end
        local.get 0
        i32.load offset=416
        local.set 6
        local.get 6
        i32.load offset=12
        i32.eqz
        br_if 1 (;@1;)
        local.get 6
        i32.load offset=76
        local.set 6
        i32.const 3435416
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 6
        i32.const 0
        i32.const 0
        i32.const 924
        call_indirect (type 5)
        br_if 1 (;@1;)
        local.get 0
        local.get 5
        i32.store offset=756
        local.get 0
        local.get 8
        f32.store offset=752
        i32.const 4525921
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3436196
          i32.const 870
          call_indirect (type 0)
          i32.const 4525921
          i32.const 1
          i32.store8
        end
        i32.const 3436196
        i32.load
        i32.load offset=92
        local.set 5
        local.get 5
        i32.load offset=8
        local.set 3
        local.get 5
        i32.load offset=12
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=352
        local.get 2
        local.get 4
        i32.store offset=356
        local.get 5
        i32.load
        local.set 4
        local.get 5
        i32.load offset=4
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=344
        local.get 2
        local.get 3
        i32.store offset=348
        local.get 2
        i32.const 272
        i32.add
        local.get 1
        i32.const 0
        i32.const 944
        call_indirect (type 2)
        i32.const 0
        i32.const 1028
        call_indirect (type 3)
        local.get 2
        f32.load offset=280
        local.set 10
        local.get 2
        f32.load offset=276
        local.set 13
        local.get 2
        f32.load offset=272
        local.set 18
        local.get 2
        i32.load offset=364
        i32.const 3470980
        i32.load
        i32.const 974
        call_indirect (type 4)
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        block  ;; label = @3
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=152
          if  ;; label = @4
            i32.const 3432484
            i32.load
            i32.const 871
            call_indirect (type 0)
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=152
            i32.load offset=12
            i32.const 49
            i32.gt_s
            br_if 1 (;@3;)
          end
          i32.const 3427900
          i32.load
          i32.const 50
          i32.const 888
          call_indirect (type 2)
          local.set 1
          i32.const 3432484
          i32.load
          i32.const 871
          call_indirect (type 0)
          i32.const 3432484
          i32.load
          i32.load offset=92
          local.get 1
          i32.store offset=152
          i32.const 3432484
          i32.load
          i32.load offset=92
          drop
        end
        local.get 0
        i32.load8_u offset=348
        if  ;; label = @3
          i32.const 0
          local.set 5
          loop  ;; label = @4
            local.get 0
            i32.load offset=416
            local.set 1
            local.get 5
            local.get 1
            i32.load offset=12
            i32.ge_s
            br_if 2 (;@2;)
            local.get 1
            local.get 5
            i32.const 164
            i32.mul
            i32.add
            i32.load offset=76
            local.set 1
            i32.const 3435416
            i32.load
            i32.const 871
            call_indirect (type 0)
            block  ;; label = @5
              local.get 1
              i32.const 0
              i32.const 0
              i32.const 915
              call_indirect (type 5)
              i32.eqz
              br_if 0 (;@5;)
              local.get 2
              i32.const 272
              i32.add
              local.get 0
              i32.load offset=416
              local.get 5
              i32.const 164
              i32.mul
              i32.add
              i32.load offset=76
              i32.const 0
              i32.const 3041
              call_indirect (type 3)
              local.get 2
              i32.load offset=288
              local.set 3
              local.get 2
              i32.load offset=292
              local.set 4
              local.get 2
              local.get 3
              i32.store offset=336
              local.get 2
              local.get 4
              i32.store offset=340
              local.get 2
              i32.load offset=280
              local.set 4
              local.get 2
              i32.load offset=284
              local.set 3
              local.get 2
              local.get 4
              i32.store offset=328
              local.get 2
              local.get 3
              i32.store offset=332
              local.get 2
              i32.load offset=272
              local.set 3
              local.get 2
              i32.load offset=276
              local.set 4
              local.get 2
              local.get 3
              i32.store offset=320
              local.get 2
              local.get 4
              i32.store offset=324
              local.get 2
              i32.load offset=320
              local.set 4
              local.get 2
              i32.load offset=324
              local.set 3
              local.get 2
              local.get 4
              i32.store offset=272
              local.get 2
              local.get 3
              i32.store offset=276
              local.get 2
              local.get 2
              i32.load offset=328
              i32.store offset=280
              local.get 2
              f32.load offset=280
              local.set 14
              local.get 2
              f32.load offset=276
              local.set 15
              local.get 2
              f32.load offset=272
              local.set 16
              i32.const 4509457
              i32.load8_u
              i32.eqz
              if  ;; label = @6
                i32.const 3434688
                i32.const 870
                call_indirect (type 0)
                i32.const 4509457
                i32.const 1
                i32.store8
              end
              local.get 16
              local.get 18
              f32.sub
              local.set 11
              local.get 15
              local.get 13
              f32.sub
              local.set 17
              local.get 14
              local.get 10
              f32.sub
              local.set 19
              i32.const 3434688
              i32.load
              i32.const 871
              call_indirect (type 0)
              local.get 2
              i32.load offset=332
              local.set 3
              local.get 2
              i32.load offset=336
              local.set 4
              local.get 2
              local.get 3
              i32.store offset=272
              local.get 2
              local.get 4
              i32.store offset=276
              local.get 2
              local.get 2
              i32.load offset=340
              i32.store offset=280
              local.get 0
              f32.load offset=336
              local.set 8
              local.get 2
              f32.load offset=272
              local.get 8
              f32.mul
              local.set 20
              local.get 2
              f32.load offset=276
              local.get 8
              f32.mul
              local.set 21
              local.get 2
              f32.load offset=280
              local.get 8
              f32.mul
              local.set 22
              i32.const 4523251
              i32.load8_u
              i32.eqz
              if  ;; label = @6
                i32.const 3434688
                i32.const 870
                call_indirect (type 0)
                i32.const 4523251
                i32.const 1
                i32.store8
              end
              local.get 18
              local.get 16
              f32.sub
              local.set 9
              local.get 13
              local.get 15
              f32.sub
              local.set 12
              local.get 10
              local.get 14
              f32.sub
              local.set 8
              local.get 11
              local.get 11
              f32.mul
              local.get 17
              local.get 17
              f32.mul
              f32.add
              local.get 19
              local.get 19
              f32.mul
              f32.add
              f32.sqrt
              local.set 17
              i32.const 3434688
              i32.load
              i32.const 871
              call_indirect (type 0)
              local.get 9
              local.get 9
              f32.mul
              local.get 12
              local.get 12
              f32.mul
              f32.add
              local.get 8
              local.get 8
              f32.mul
              f32.add
              f32.sqrt
              local.set 11
              block  ;; label = @6
                local.get 11
                f32.const 0x1.4f8b58p-17 (;=1e-05;)
                f32.gt
                if  ;; label = @7
                  local.get 8
                  local.get 11
                  f32.div
                  local.set 8
                  local.get 9
                  local.get 11
                  f32.div
                  local.set 9
                  local.get 12
                  local.get 11
                  f32.div
                  local.set 12
                  br 1 (;@6;)
                end
                i32.const 4525920
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3439908
                  i32.const 870
                  call_indirect (type 0)
                  i32.const 4525920
                  i32.const 1
                  i32.store8
                end
                i32.const 3439908
                i32.load
                i32.load offset=92
                local.set 1
                local.get 1
                f32.load offset=8
                local.set 8
                local.get 1
                f32.load
                local.set 9
                local.get 1
                f32.load offset=4
                local.set 12
              end
              i32.const 0
              local.set 1
              i32.const 3432484
              i32.load
              i32.const 871
              call_indirect (type 0)
              i32.const 3432484
              i32.load
              i32.load offset=92
              i32.load offset=152
              local.set 6
              local.get 0
              i32.load offset=332
              local.set 7
              local.get 2
              local.get 14
              f32.store offset=264
              local.get 2
              local.get 22
              f32.store offset=248
              local.get 2
              local.get 8
              f32.store offset=232
              local.get 2
              local.get 2
              i32.load offset=264
              i32.store offset=144
              local.get 2
              local.get 2
              i32.load offset=248
              i32.store offset=128
              local.get 2
              local.get 2
              i32.load offset=232
              i32.store offset=112
              local.get 2
              local.get 15
              f32.store offset=260
              local.get 2
              local.get 16
              f32.store offset=256
              local.get 2
              local.get 21
              f32.store offset=244
              local.get 2
              local.get 20
              f32.store offset=240
              local.get 2
              local.get 12
              f32.store offset=228
              local.get 2
              local.get 9
              f32.store offset=224
              local.get 2
              i32.load offset=256
              local.set 4
              local.get 2
              i32.load offset=260
              local.set 3
              local.get 2
              local.get 4
              i32.store offset=136
              local.get 2
              local.get 3
              i32.store offset=140
              local.get 2
              i32.load offset=240
              local.set 3
              local.get 2
              i32.load offset=244
              local.set 4
              local.get 2
              local.get 3
              i32.store offset=120
              local.get 2
              local.get 4
              i32.store offset=124
              local.get 2
              i32.load offset=224
              local.set 4
              local.get 2
              i32.load offset=228
              local.set 3
              local.get 2
              local.get 4
              i32.store offset=104
              local.get 2
              local.get 3
              i32.store offset=108
              local.get 2
              i32.load offset=352
              local.set 3
              local.get 2
              i32.load offset=356
              local.set 4
              local.get 2
              local.get 3
              i32.store offset=96
              local.get 2
              local.get 4
              i32.store offset=100
              local.get 2
              i32.load offset=344
              local.set 4
              local.get 2
              i32.load offset=348
              local.set 3
              local.get 2
              local.get 4
              i32.store offset=88
              local.get 2
              local.get 3
              i32.store offset=92
              local.get 2
              i32.const 136
              i32.add
              local.get 2
              i32.const 120
              i32.add
              local.get 2
              i32.const 104
              i32.add
              local.get 6
              local.get 2
              i32.const 88
              i32.add
              local.get 17
              local.get 7
              i32.const 0
              call $j19075 ;; 
              local.set 7
              loop  ;; label = @6
                local.get 1
                local.get 7
                i32.ge_s
                br_if 1 (;@5;)
                i32.const 3432484
                i32.load
                i32.const 871
                call_indirect (type 0)
                i32.const 3432484
                i32.load
                i32.load offset=92
                i32.load offset=152
                local.get 1
                i32.const 44
                i32.mul
                i32.add
                i32.const 16
                i32.add
                i32.const 0
                i32.const 2790
                call_indirect (type 2)
                i32.const 3485768
                i32.load
                i32.const 3390
                call_indirect (type 2)
                local.set 6
                i32.const 3435416
                i32.load
                i32.const 871
                call_indirect (type 0)
                block  ;; label = @7
                  local.get 6
                  i32.const 0
                  i32.const 0
                  i32.const 915
                  call_indirect (type 5)
                  i32.eqz
                  br_if 0 (;@7;)
                  local.get 2
                  i32.load offset=364
                  local.get 6
                  i32.const 3470984
                  i32.load
                  i32.const 1235
                  call_indirect (type 5)
                  br_if 0 (;@7;)
                  local.get 2
                  i32.load offset=364
                  local.get 6
                  i32.const 3470972
                  i32.load
                  i32.const 903
                  call_indirect (type 3)
                end
                local.get 1
                i32.const 1
                i32.add
                local.set 1
                br 0 (;@6;)
              end
              unreachable
            end
            local.get 5
            i32.const 1
            i32.add
            local.set 5
            br 0 (;@4;)
          end
          unreachable
        end
        local.get 2
        i32.const 272
        i32.add
        local.get 0
        i32.load offset=416
        i32.load offset=76
        i32.const 0
        i32.const 3041
        call_indirect (type 3)
        local.get 2
        i32.load offset=288
        local.set 3
        local.get 2
        i32.load offset=292
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=312
        local.get 2
        local.get 4
        i32.store offset=316
        local.get 2
        i32.load offset=280
        local.set 4
        local.get 2
        i32.load offset=284
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=304
        local.get 2
        local.get 3
        i32.store offset=308
        local.get 2
        i32.load offset=272
        local.set 3
        local.get 2
        i32.load offset=276
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=296
        local.get 2
        local.get 4
        i32.store offset=300
        i32.const 1
        local.set 1
        loop  ;; label = @3
          local.get 0
          i32.load offset=416
          local.set 5
          local.get 5
          i32.load offset=12
          local.get 1
          i32.gt_s
          if  ;; label = @4
            local.get 5
            local.get 1
            i32.const 164
            i32.mul
            i32.add
            i32.load offset=76
            local.set 5
            i32.const 3435416
            i32.load
            i32.const 871
            call_indirect (type 0)
            local.get 5
            i32.const 0
            i32.const 0
            i32.const 915
            call_indirect (type 5)
            if  ;; label = @5
              local.get 2
              i32.const 272
              i32.add
              local.get 0
              i32.load offset=416
              local.get 1
              i32.const 164
              i32.mul
              i32.add
              i32.load offset=76
              i32.const 0
              i32.const 3041
              call_indirect (type 3)
              local.get 2
              i32.load offset=288
              local.set 4
              local.get 2
              i32.load offset=292
              local.set 3
              local.get 4
              local.set 6
              local.get 3
              local.set 5
              local.get 2
              local.get 6
              i32.store offset=216
              local.get 2
              local.get 3
              i32.store offset=220
              local.get 2
              i32.load offset=280
              local.set 3
              local.get 2
              i32.load offset=284
              local.set 4
              local.get 2
              local.get 3
              i32.store offset=208
              local.get 2
              local.get 4
              i32.store offset=212
              local.get 2
              local.get 3
              i32.store offset=8
              local.get 2
              local.get 4
              i32.store offset=12
              local.get 5
              local.set 4
              local.get 2
              local.get 6
              i32.store offset=16
              local.get 2
              local.get 4
              i32.store offset=20
              local.get 2
              i32.load offset=272
              local.set 4
              local.get 2
              i32.load offset=276
              local.set 3
              local.get 4
              local.set 6
              local.get 2
              local.get 6
              i32.store offset=200
              local.get 2
              local.get 3
              i32.store offset=204
              local.get 2
              local.get 6
              i32.store
              local.get 2
              local.get 3
              i32.store offset=4
              local.get 2
              i32.const 296
              i32.add
              local.get 2
              i32.const 0
              i32.const 6924
              call_indirect (type 3)
            end
            local.get 1
            i32.const 1
            i32.add
            local.set 1
            br 1 (;@3;)
          end
        end
        local.get 2
        i32.load offset=296
        local.set 3
        local.get 2
        i32.load offset=300
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=272
        local.get 2
        local.get 4
        i32.store offset=276
        local.get 2
        local.get 2
        i32.load offset=304
        i32.store offset=280
        local.get 2
        f32.load offset=280
        local.set 14
        local.get 2
        f32.load offset=276
        local.set 15
        local.get 2
        f32.load offset=272
        local.set 16
        i32.const 4509457
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3434688
          i32.const 870
          call_indirect (type 0)
          i32.const 4509457
          i32.const 1
          i32.store8
        end
        local.get 16
        local.get 18
        f32.sub
        local.set 12
        local.get 15
        local.get 13
        f32.sub
        local.set 11
        local.get 14
        local.get 10
        f32.sub
        local.set 17
        i32.const 3434688
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 2
        i32.load offset=308
        local.set 4
        local.get 2
        i32.load offset=312
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=272
        local.get 2
        local.get 3
        i32.store offset=276
        local.get 2
        local.get 2
        i32.load offset=316
        i32.store offset=280
        local.get 0
        f32.load offset=336
        local.set 8
        local.get 2
        f32.load offset=272
        local.get 8
        f32.mul
        local.set 19
        local.get 2
        f32.load offset=276
        local.get 8
        f32.mul
        local.set 20
        local.get 2
        f32.load offset=280
        local.get 8
        f32.mul
        local.set 21
        i32.const 4523251
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3434688
          i32.const 870
          call_indirect (type 0)
          i32.const 4523251
          i32.const 1
          i32.store8
        end
        local.get 18
        local.get 16
        f32.sub
        local.set 9
        local.get 13
        local.get 15
        f32.sub
        local.set 13
        local.get 10
        local.get 14
        f32.sub
        local.set 8
        local.get 12
        local.get 12
        f32.mul
        local.get 11
        local.get 11
        f32.mul
        f32.add
        local.get 17
        local.get 17
        f32.mul
        f32.add
        f32.sqrt
        local.set 18
        i32.const 3434688
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 9
        local.get 9
        f32.mul
        local.get 13
        local.get 13
        f32.mul
        f32.add
        local.get 8
        local.get 8
        f32.mul
        f32.add
        f32.sqrt
        local.set 10
        block  ;; label = @3
          local.get 10
          f32.const 0x1.4f8b58p-17 (;=1e-05;)
          f32.gt
          if  ;; label = @4
            local.get 8
            local.get 10
            f32.div
            local.set 8
            local.get 9
            local.get 10
            f32.div
            local.set 9
            local.get 13
            local.get 10
            f32.div
            local.set 10
            br 1 (;@3;)
          end
          i32.const 4525920
          i32.load8_u
          i32.eqz
          if  ;; label = @4
            i32.const 3439908
            i32.const 870
            call_indirect (type 0)
            i32.const 4525920
            i32.const 1
            i32.store8
          end
          i32.const 3439908
          i32.load
          i32.load offset=92
          local.set 1
          local.get 1
          f32.load offset=8
          local.set 8
          local.get 1
          f32.load
          local.set 9
          local.get 1
          f32.load offset=4
          local.set 10
        end
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=152
        local.set 1
        local.get 2
        i32.load offset=352
        local.set 3
        local.get 2
        i32.load offset=356
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=280
        local.get 2
        local.get 4
        i32.store offset=284
        local.get 2
        i32.load offset=344
        local.set 4
        local.get 2
        i32.load offset=348
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=272
        local.get 2
        local.get 3
        i32.store offset=276
        local.get 0
        i32.load offset=332
        local.set 5
        local.get 2
        local.get 14
        f32.store offset=192
        local.get 2
        local.get 21
        f32.store offset=176
        local.get 2
        local.get 8
        f32.store offset=160
        local.get 2
        local.get 2
        i32.load offset=192
        i32.store offset=80
        local.get 2
        i32.const -64
        i32.sub
        local.get 2
        i32.load offset=176
        i32.store
        local.get 2
        local.get 2
        i32.load offset=160
        i32.store offset=48
        local.get 2
        local.get 15
        f32.store offset=188
        local.get 2
        local.get 16
        f32.store offset=184
        local.get 2
        local.get 20
        f32.store offset=172
        local.get 2
        local.get 19
        f32.store offset=168
        local.get 2
        local.get 10
        f32.store offset=156
        local.get 2
        local.get 9
        f32.store offset=152
        local.get 2
        i32.load offset=184
        local.set 3
        local.get 2
        i32.load offset=188
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=72
        local.get 2
        local.get 4
        i32.store offset=76
        local.get 2
        i32.load offset=168
        local.set 4
        local.get 2
        i32.load offset=172
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=56
        local.get 2
        local.get 3
        i32.store offset=60
        local.get 2
        i32.load offset=152
        local.set 3
        local.get 2
        i32.load offset=156
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=40
        local.get 2
        local.get 4
        i32.store offset=44
        local.get 2
        i32.load offset=280
        local.set 4
        local.get 2
        i32.load offset=284
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=32
        local.get 2
        local.get 3
        i32.store offset=36
        local.get 2
        i32.load offset=272
        local.set 3
        local.get 2
        i32.load offset=276
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=24
        local.get 2
        local.get 4
        i32.store offset=28
        local.get 2
        i32.const 72
        i32.add
        local.get 2
        i32.const 56
        i32.add
        local.get 2
        i32.const 40
        i32.add
        local.get 1
        local.get 2
        i32.const 24
        i32.add
        local.get 18
        local.get 5
        i32.const 0
        call $j19075 ;; 
        local.set 6
        i32.const 0
        local.set 1
        loop  ;; label = @3
          local.get 1
          local.get 6
          i32.ge_s
          br_if 1 (;@2;)
          i32.const 3432484
          i32.load
          i32.const 871
          call_indirect (type 0)
          i32.const 3432484
          i32.load
          i32.load offset=92
          i32.load offset=152
          local.get 1
          i32.const 44
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.const 0
          i32.const 2790
          call_indirect (type 2)
          i32.const 3485768
          i32.load
          i32.const 3390
          call_indirect (type 2)
          local.set 5
          i32.const 3435416
          i32.load
          i32.const 871
          call_indirect (type 0)
          local.get 5
          i32.const 0
          i32.const 0
          i32.const 915
          call_indirect (type 5)
          if  ;; label = @4
            local.get 2
            i32.load offset=364
            local.get 5
            i32.const 3470972
            i32.load
            i32.const 903
            call_indirect (type 3)
          end
          local.get 1
          i32.const 1
          i32.add
          local.set 1
          br 0 (;@3;)
        end
        unreachable
      end
      i32.const 3470992
      i32.load
      drop
      local.get 2
      i32.load offset=364
      i32.load offset=12
      local.set 6
      local.get 6
      i32.const 1
      i32.lt_s
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=484
      i32.const 0
      i32.const 3233
      call_indirect (type 4)
      i32.const 0
      local.set 1
      loop  ;; label = @2
        local.get 1
        local.get 6
        i32.lt_s
        if  ;; label = @3
          local.get 2
          i32.load offset=364
          local.get 1
          i32.const 3470996
          i32.load
          i32.const 895
          call_indirect (type 5)
          local.set 5
          i32.const 3435416
          i32.load
          i32.const 871
          call_indirect (type 0)
          local.get 5
          i32.const 0
          i32.const 0
          i32.const 915
          call_indirect (type 5)
          if  ;; label = @4
            local.get 0
            i32.load offset=484
            local.set 7
            i32.const 3432484
            i32.load
            i32.const 871
            call_indirect (type 0)
            local.get 7
            local.get 5
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=56
            i32.const 0
            call $j42588 ;; 
          end
          local.get 1
          i32.const 1
          i32.add
          local.set 1
          br 1 (;@2;)
        end
      end
      local.get 0
      i32.load offset=484
      local.set 0
      i32.const 3432324
      i32.load
      i32.const 871
      call_indirect (type 0)
      local.get 0
      i32.const 0
      i32.const 3211
      call_indirect (type 4)
    end
    local.get 2
    i32.const 368
    i32.add
    global.set 0)
