  (func $j39449 (type 5) (param i32 i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 352
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4505063
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505063
      i32.const 1
      i32.store8
    end
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
    i32.const 1519
    call_indirect (type 21)
    local.set 8
    i32.const 0
    i32.const 2109
    call_indirect (type 1)
    local.set 5
    block  ;; label = @1
      block  ;; label = @2
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
        br_if 0 (;@2;)
        i32.const 0
        i32.const 1337
        call_indirect (type 1)
        i32.eqz
        br_if 0 (;@2;)
        local.get 0
        i32.load offset=756
        local.get 5
        i32.eq
        br_if 0 (;@2;)
        local.get 0
        i32.load8_u offset=760
        i32.const 0
        i32.ne
        local.set 5
        br 1 (;@1;)
      end
      local.get 0
      local.get 5
      i32.store offset=756
      local.get 0
      local.get 8
      f32.store offset=752
      local.get 0
      i32.load offset=416
      local.set 4
      local.get 4
      i32.load offset=12
      i32.eqz
      if  ;; label = @2
        i32.const 0
        local.set 5
        br 1 (;@1;)
      end
      i32.const 0
      local.set 5
      local.get 4
      i32.const 16
      i32.add
      i32.load offset=60
      local.set 4
      i32.const 3435416
      i32.load
      i32.const 871
      call_indirect (type 0)
      local.get 4
      i32.const 0
      i32.const 0
      i32.const 924
      call_indirect (type 5)
      br_if 0 (;@1;)
      local.get 2
      i32.const 280
      i32.add
      local.get 1
      i32.const 0
      i32.const 944
      call_indirect (type 2)
      i32.const 0
      i32.const 1028
      call_indirect (type 3)
      local.get 2
      f32.load offset=288
      local.set 11
      local.get 2
      f32.load offset=284
      local.set 12
      local.get 2
      f32.load offset=280
      local.set 17
      local.get 0
      local.set 7
      block  ;; label = @2
        local.get 0
        i32.load8_u offset=348
        if  ;; label = @3
          loop  ;; label = @4
            local.get 0
            i32.load offset=416
            local.set 1
            local.get 1
            i32.load offset=12
            local.get 5
            i32.gt_s
            if  ;; label = @5
              local.get 1
              local.get 5
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              i32.load offset=60
              local.set 1
              i32.const 3435416
              i32.load
              i32.const 871
              call_indirect (type 0)
              block  ;; label = @6
                local.get 1
                i32.const 0
                i32.const 0
                i32.const 915
                call_indirect (type 5)
                i32.eqz
                br_if 0 (;@6;)
                local.get 2
                i32.const 280
                i32.add
                local.get 0
                i32.load offset=416
                local.get 5
                i32.const 164
                i32.mul
                i32.add
                i32.const 16
                i32.add
                i32.load offset=60
                i32.const 0
                i32.const 3041
                call_indirect (type 3)
                local.get 2
                i32.load offset=296
                local.set 3
                local.get 2
                i32.load offset=300
                local.set 4
                local.get 2
                local.get 3
                i32.store offset=344
                local.get 2
                local.get 4
                i32.store offset=348
                local.get 2
                i32.load offset=288
                local.set 4
                local.get 2
                i32.load offset=292
                local.set 3
                local.get 2
                local.get 4
                i32.store offset=336
                local.get 2
                local.get 3
                i32.store offset=340
                local.get 2
                i32.load offset=280
                local.set 3
                local.get 2
                i32.load offset=284
                local.set 4
                local.get 2
                local.get 3
                i32.store offset=328
                local.get 2
                local.get 4
                i32.store offset=332
                local.get 2
                i32.load offset=328
                local.set 4
                local.get 2
                i32.load offset=332
                local.set 3
                local.get 2
                local.get 4
                i32.store offset=280
                local.get 2
                local.get 3
                i32.store offset=284
                local.get 2
                local.get 2
                i32.load offset=336
                i32.store offset=288
                local.get 2
                f32.load offset=288
                local.set 13
                local.get 2
                f32.load offset=284
                local.set 14
                local.get 2
                f32.load offset=280
                local.set 15
                i32.const 4509457
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3434688
                  i32.const 870
                  call_indirect (type 0)
                  i32.const 4509457
                  i32.const 1
                  i32.store8
                end
                local.get 15
                local.get 17
                f32.sub
                local.set 8
                local.get 14
                local.get 12
                f32.sub
                local.set 9
                local.get 13
                local.get 11
                f32.sub
                local.set 10
                i32.const 3434688
                i32.load
                i32.const 871
                call_indirect (type 0)
                local.get 8
                local.get 8
                f32.mul
                local.get 9
                local.get 9
                f32.mul
                f32.add
                local.get 10
                local.get 10
                f32.mul
                f32.add
                local.set 18
                local.get 2
                i32.load offset=340
                local.set 3
                local.get 2
                i32.load offset=344
                local.set 4
                local.get 2
                local.get 3
                i32.store offset=280
                local.get 2
                local.get 4
                i32.store offset=284
                local.get 2
                local.get 2
                i32.load offset=348
                i32.store offset=288
                local.get 0
                f32.load offset=336
                local.set 8
                local.get 2
                f32.load offset=280
                local.get 8
                f32.mul
                local.set 19
                local.get 2
                f32.load offset=284
                local.get 8
                f32.mul
                local.set 20
                local.get 2
                f32.load offset=288
                local.get 8
                f32.mul
                local.set 21
                local.get 17
                local.get 15
                f32.sub
                local.set 9
                local.get 12
                local.get 14
                f32.sub
                local.set 10
                local.get 11
                local.get 13
                f32.sub
                local.set 8
                i32.const 4523251
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3434688
                  i32.const 870
                  call_indirect (type 0)
                  i32.const 4523251
                  i32.const 1
                  i32.store8
                end
                i32.const 3434688
                i32.load
                i32.const 871
                call_indirect (type 0)
                local.get 9
                local.get 9
                f32.mul
                local.get 10
                local.get 10
                f32.mul
                f32.add
                local.get 8
                local.get 8
                f32.mul
                f32.add
                f32.sqrt
                local.set 16
                block  ;; label = @7
                  local.get 16
                  f32.const 0x1.4f8b58p-17 (;=1e-05;)
                  f32.gt
                  if  ;; label = @8
                    local.get 8
                    local.get 16
                    f32.div
                    local.set 8
                    local.get 9
                    local.get 16
                    f32.div
                    local.set 9
                    local.get 10
                    local.get 16
                    f32.div
                    local.set 10
                    br 1 (;@7;)
                  end
                  i32.const 4525920
                  i32.load8_u
                  i32.eqz
                  if  ;; label = @8
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
                i32.const 4525921
                i32.load8_u
                i32.eqz
                if  ;; label = @7
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
                local.set 1
                local.get 1
                i32.load offset=8
                local.set 4
                local.get 1
                i32.load offset=12
                local.set 3
                local.get 2
                local.get 4
                i32.store offset=288
                local.get 2
                local.get 3
                i32.store offset=292
                local.get 1
                i32.load
                local.set 3
                local.get 1
                i32.load offset=4
                local.set 4
                local.get 2
                local.get 3
                i32.store offset=280
                local.get 2
                local.get 4
                i32.store offset=284
                local.get 0
                i32.load offset=332
                local.set 1
                local.get 2
                local.get 13
                f32.store offset=272
                local.get 2
                local.get 21
                f32.store offset=256
                local.get 2
                local.get 8
                f32.store offset=240
                local.get 2
                local.get 2
                i32.load offset=272
                i32.store offset=152
                local.get 2
                local.get 2
                i32.load offset=256
                i32.store offset=136
                local.get 2
                local.get 2
                i32.load offset=240
                i32.store offset=120
                local.get 2
                local.get 14
                f32.store offset=268
                local.get 2
                local.get 15
                f32.store offset=264
                local.get 2
                local.get 20
                f32.store offset=252
                local.get 2
                local.get 19
                f32.store offset=248
                local.get 2
                local.get 10
                f32.store offset=236
                local.get 2
                local.get 9
                f32.store offset=232
                local.get 2
                i32.load offset=264
                local.set 4
                local.get 2
                i32.load offset=268
                local.set 3
                local.get 2
                local.get 4
                i32.store offset=144
                local.get 2
                local.get 3
                i32.store offset=148
                local.get 2
                i32.load offset=248
                local.set 3
                local.get 2
                i32.load offset=252
                local.set 4
                local.get 2
                local.get 3
                i32.store offset=128
                local.get 2
                local.get 4
                i32.store offset=132
                local.get 2
                i32.load offset=232
                local.set 4
                local.get 2
                i32.load offset=236
                local.set 3
                local.get 2
                local.get 4
                i32.store offset=112
                local.get 2
                local.get 3
                i32.store offset=116
                local.get 2
                i32.load offset=288
                local.set 3
                local.get 2
                i32.load offset=292
                local.set 4
                local.get 2
                local.get 3
                i32.store offset=104
                local.get 2
                local.get 4
                i32.store offset=108
                local.get 2
                i32.load offset=280
                local.set 4
                local.get 2
                i32.load offset=284
                local.set 3
                local.get 2
                local.get 4
                i32.store offset=96
                local.get 2
                local.get 3
                i32.store offset=100
                local.get 2
                i32.const 144
                i32.add
                local.get 2
                i32.const 128
                i32.add
                local.get 2
                i32.const 112
                i32.add
                local.get 2
                i32.const 96
                i32.add
                local.get 18
                f32.sqrt
                local.get 1
                i32.const 0
                call $j19073 ;; 
                i32.eqz
                br_if 0 (;@6;)
                i32.const 1
                local.set 5
                br 4 (;@2;)
              end
              local.get 5
              i32.const 1
              i32.add
              local.set 5
              br 1 (;@4;)
            end
          end
          i32.const 0
          local.set 5
          br 1 (;@2;)
        end
        local.get 2
        i32.const 280
        i32.add
        local.get 0
        i32.load offset=416
        i32.const 16
        i32.add
        i32.load offset=60
        i32.const 0
        i32.const 3041
        call_indirect (type 3)
        local.get 2
        i32.load offset=296
        local.set 3
        local.get 2
        i32.load offset=300
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=320
        local.get 2
        local.get 4
        i32.store offset=324
        local.get 2
        i32.load offset=288
        local.set 4
        local.get 2
        i32.load offset=292
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=312
        local.get 2
        local.get 3
        i32.store offset=316
        local.get 2
        i32.load offset=280
        local.set 3
        local.get 2
        i32.load offset=284
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=304
        local.get 2
        local.get 4
        i32.store offset=308
        i32.const 1
        local.set 5
        loop  ;; label = @3
          local.get 0
          i32.load offset=416
          local.set 1
          local.get 1
          i32.load offset=12
          local.get 5
          i32.gt_s
          if  ;; label = @4
            local.get 1
            local.get 5
            i32.const 164
            i32.mul
            i32.add
            i32.const 16
            i32.add
            i32.load offset=60
            local.set 1
            i32.const 3435416
            i32.load
            i32.const 871
            call_indirect (type 0)
            local.get 1
            i32.const 0
            i32.const 0
            i32.const 915
            call_indirect (type 5)
            if  ;; label = @5
              local.get 2
              i32.const 280
              i32.add
              local.get 0
              i32.load offset=416
              local.get 5
              i32.const 164
              i32.mul
              i32.add
              i32.const 16
              i32.add
              i32.load offset=60
              i32.const 0
              i32.const 3041
              call_indirect (type 3)
              local.get 2
              i32.load offset=296
              local.set 4
              local.get 2
              i32.load offset=300
              local.set 3
              local.get 4
              local.set 6
              local.get 3
              local.set 1
              local.get 2
              local.get 6
              i32.store offset=224
              local.get 2
              local.get 3
              i32.store offset=228
              local.get 2
              i32.load offset=288
              local.set 3
              local.get 2
              i32.load offset=292
              local.set 4
              local.get 2
              local.get 3
              i32.store offset=216
              local.get 2
              local.get 4
              i32.store offset=220
              local.get 2
              local.get 3
              i32.store offset=16
              local.get 2
              local.get 4
              i32.store offset=20
              local.get 1
              local.set 4
              local.get 2
              local.get 6
              i32.store offset=24
              local.get 2
              local.get 4
              i32.store offset=28
              local.get 2
              i32.load offset=280
              local.set 4
              local.get 2
              i32.load offset=284
              local.set 3
              local.get 4
              local.set 6
              local.get 2
              local.get 6
              i32.store offset=208
              local.get 2
              local.get 3
              i32.store offset=212
              local.get 2
              local.get 6
              i32.store offset=8
              local.get 2
              local.get 3
              i32.store offset=12
              local.get 2
              i32.const 304
              i32.add
              local.get 2
              i32.const 8
              i32.add
              i32.const 0
              i32.const 6924
              call_indirect (type 3)
            end
            local.get 5
            i32.const 1
            i32.add
            local.set 5
            br 1 (;@3;)
          end
        end
        local.get 2
        i32.load offset=304
        local.set 3
        local.get 2
        i32.load offset=308
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=280
        local.get 2
        local.get 4
        i32.store offset=284
        local.get 2
        local.get 2
        i32.load offset=312
        i32.store offset=288
        local.get 2
        f32.load offset=288
        local.set 13
        local.get 2
        f32.load offset=284
        local.set 14
        local.get 2
        f32.load offset=280
        local.set 15
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
        local.get 15
        local.get 17
        f32.sub
        local.set 8
        local.get 14
        local.get 12
        f32.sub
        local.set 9
        local.get 13
        local.get 11
        f32.sub
        local.set 10
        i32.const 3434688
        i32.load
        i32.const 871
        call_indirect (type 0)
        local.get 8
        local.get 8
        f32.mul
        local.get 9
        local.get 9
        f32.mul
        f32.add
        local.get 10
        local.get 10
        f32.mul
        f32.add
        local.set 10
        local.get 2
        i32.load offset=316
        local.set 4
        local.get 2
        i32.load offset=320
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=280
        local.get 2
        local.get 3
        i32.store offset=284
        local.get 2
        local.get 2
        i32.load offset=324
        i32.store offset=288
        local.get 0
        f32.load offset=336
        local.set 8
        local.get 2
        f32.load offset=280
        local.get 8
        f32.mul
        local.set 16
        local.get 2
        f32.load offset=284
        local.get 8
        f32.mul
        local.set 18
        local.get 2
        f32.load offset=288
        local.get 8
        f32.mul
        local.set 19
        local.get 17
        local.get 15
        f32.sub
        local.set 9
        local.get 12
        local.get 14
        f32.sub
        local.set 12
        local.get 11
        local.get 13
        f32.sub
        local.set 8
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
        block  ;; label = @3
          local.get 11
          f32.const 0x1.4f8b58p-17 (;=1e-05;)
          f32.gt
          if  ;; label = @4
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
            local.set 11
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
          local.set 11
        end
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
        local.set 1
        local.get 1
        i32.load offset=8
        local.set 3
        local.get 1
        i32.load offset=12
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=288
        local.get 2
        local.get 4
        i32.store offset=292
        local.get 1
        i32.load
        local.set 4
        local.get 1
        i32.load offset=4
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=280
        local.get 2
        local.get 3
        i32.store offset=284
        local.get 0
        i32.load offset=332
        local.set 0
        local.get 2
        local.get 13
        f32.store offset=200
        local.get 2
        local.get 19
        f32.store offset=184
        local.get 2
        local.get 8
        f32.store offset=168
        local.get 2
        local.get 2
        i32.load offset=200
        i32.store offset=88
        local.get 2
        local.get 2
        i32.load offset=184
        i32.store offset=72
        local.get 2
        local.get 2
        i32.load offset=168
        i32.store offset=56
        local.get 2
        local.get 14
        f32.store offset=196
        local.get 2
        local.get 15
        f32.store offset=192
        local.get 2
        local.get 18
        f32.store offset=180
        local.get 2
        local.get 16
        f32.store offset=176
        local.get 2
        local.get 11
        f32.store offset=164
        local.get 2
        local.get 9
        f32.store offset=160
        local.get 2
        i32.load offset=192
        local.set 3
        local.get 2
        i32.load offset=196
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=80
        local.get 2
        local.get 4
        i32.store offset=84
        local.get 2
        i32.load offset=176
        local.set 4
        local.get 2
        i32.load offset=180
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=64
        local.get 2
        local.get 3
        i32.store offset=68
        local.get 2
        i32.load offset=160
        local.set 3
        local.get 2
        i32.load offset=164
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=48
        local.get 2
        local.get 4
        i32.store offset=52
        local.get 2
        i32.load offset=288
        local.set 4
        local.get 2
        i32.load offset=292
        local.set 3
        local.get 2
        local.get 4
        i32.store offset=40
        local.get 2
        local.get 3
        i32.store offset=44
        local.get 2
        i32.load offset=280
        local.set 3
        local.get 2
        i32.load offset=284
        local.set 4
        local.get 2
        local.get 3
        i32.store offset=32
        local.get 2
        local.get 4
        i32.store offset=36
        local.get 2
        i32.const 80
        i32.add
        local.get 2
        i32.const -64
        i32.sub
        local.get 2
        i32.const 48
        i32.add
        local.get 2
        i32.const 32
        i32.add
        local.get 10
        f32.sqrt
        local.get 0
        i32.const 0
        call $j19073 ;; 
        local.set 5
      end
      local.get 7
      local.get 5
      i32.store8 offset=760
    end
    local.get 2
    i32.const 352
    i32.add
    global.set 0
    local.get 5
    return)
