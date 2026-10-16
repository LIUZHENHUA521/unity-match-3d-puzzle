  (func $j39440 (type 5) (param i32 i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 224
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4505059
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      i32.const 870
      call_indirect (type 0)
      i32.const 3468552
      i32.const 870
      call_indirect (type 0)
      i32.const 3468556
      i32.const 870
      call_indirect (type 0)
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 3427900
      i32.const 870
      call_indirect (type 0)
      i32.const 4505059
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 4
    local.get 2
    i32.const 0
    i32.store offset=216
    local.get 2
    local.get 4
    i32.store offset=220
    i32.const 0
    local.set 4
    local.get 2
    i32.const 0
    i32.store offset=208
    local.get 2
    local.get 4
    i32.store offset=212
    i32.const 0
    local.set 4
    local.get 2
    i32.const 0
    i32.store offset=200
    local.get 2
    local.get 4
    i32.store offset=204
    local.get 2
    i32.const 200
    i32.add
    i32.const 24
    i32.const 897
    call_indirect (type 4)
    loop  ;; label = @1
      local.get 0
      i32.load offset=416
      local.set 3
      local.get 3
      i32.load offset=12
      local.get 5
      i32.gt_s
      if  ;; label = @2
        local.get 3
        local.get 5
        i32.const 164
        i32.mul
        i32.add
        i32.const 16
        i32.add
        i32.load offset=60
        local.set 3
        i32.const 3435416
        i32.load
        i32.const 871
        call_indirect (type 0)
        block  ;; label = @3
          local.get 3
          i32.const 0
          i32.const 0
          i32.const 915
          call_indirect (type 5)
          i32.eqz
          br_if 0 (;@3;)
          local.get 2
          i32.const 176
          i32.add
          local.get 2
          i32.const 200
          i32.add
          i32.const 0
          i32.const 1675
          call_indirect (type 3)
          local.get 0
          i32.load offset=416
          local.get 5
          i32.const 164
          i32.mul
          i32.add
          i32.const 16
          i32.add
          i32.load offset=60
          local.set 3
          local.get 2
          f32.load offset=176
          f32.const 0x0p+0 (;=0;)
          f32.eq
          if  ;; label = @4
            local.get 2
            i32.const 176
            i32.add
            local.get 3
            i32.const 0
            i32.const 3041
            call_indirect (type 3)
            local.get 2
            i32.load offset=192
            local.set 4
            local.get 2
            i32.load offset=196
            local.set 3
            local.get 2
            local.get 4
            i32.store offset=216
            local.get 2
            local.get 3
            i32.store offset=220
            local.get 2
            i32.load offset=184
            local.set 3
            local.get 2
            i32.load offset=188
            local.set 4
            local.get 2
            local.get 3
            i32.store offset=208
            local.get 2
            local.get 4
            i32.store offset=212
            local.get 2
            i32.load offset=176
            local.set 4
            local.get 2
            i32.load offset=180
            local.set 3
            local.get 2
            local.get 4
            i32.store offset=200
            local.get 2
            local.get 3
            i32.store offset=204
            br 1 (;@3;)
          end
          local.get 2
          i32.const 176
          i32.add
          local.get 3
          i32.const 0
          i32.const 3041
          call_indirect (type 3)
          local.get 2
          i32.load offset=192
          local.set 3
          local.get 2
          i32.load offset=196
          local.set 4
          local.get 3
          local.set 6
          local.get 4
          local.set 7
          local.get 2
          local.get 6
          i32.store offset=168
          local.get 2
          local.get 4
          i32.store offset=172
          local.get 2
          i32.load offset=184
          local.set 4
          local.get 2
          i32.load offset=188
          local.set 3
          local.get 2
          local.get 4
          i32.store offset=160
          local.get 2
          local.get 3
          i32.store offset=164
          local.get 2
          local.get 4
          i32.store offset=8
          local.get 2
          local.get 3
          i32.store offset=12
          local.get 7
          local.set 3
          local.get 2
          local.get 6
          i32.store offset=16
          local.get 2
          local.get 3
          i32.store offset=20
          local.get 2
          i32.load offset=176
          local.set 3
          local.get 2
          i32.load offset=180
          local.set 4
          local.get 3
          local.set 6
          local.get 2
          local.get 6
          i32.store offset=152
          local.get 2
          local.get 4
          i32.store offset=156
          local.get 2
          local.get 6
          i32.store
          local.get 2
          local.get 4
          i32.store offset=4
          local.get 2
          i32.const 200
          i32.add
          local.get 2
          i32.const 0
          i32.const 6924
          call_indirect (type 3)
        end
        local.get 5
        i32.const 1
        i32.add
        local.set 5
        br 1 (;@1;)
      end
    end
    local.get 2
    i32.load offset=200
    local.set 4
    local.get 2
    i32.load offset=204
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=176
    local.get 2
    local.get 3
    i32.store offset=180
    local.get 2
    local.get 2
    i32.load offset=208
    i32.store offset=184
    local.get 2
    f32.load offset=184
    local.set 8
    local.get 2
    f32.load offset=180
    local.set 15
    local.get 2
    f32.load offset=176
    local.set 16
    local.get 2
    i32.const 176
    i32.add
    local.get 1
    i32.const 0
    i32.const 944
    call_indirect (type 2)
    i32.const 0
    i32.const 1028
    call_indirect (type 3)
    local.get 2
    f32.load offset=176
    local.set 9
    local.get 16
    local.get 9
    f32.sub
    local.set 10
    local.get 2
    f32.load offset=180
    local.set 11
    local.get 15
    local.get 11
    f32.sub
    local.set 12
    local.get 2
    f32.load offset=184
    local.set 13
    local.get 8
    local.get 13
    f32.sub
    local.set 14
    i32.const 4509457
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      i32.const 870
      call_indirect (type 0)
      i32.const 4509457
      i32.const 1
      i32.store8
    end
    local.get 16
    local.get 9
    f32.sub
    local.set 9
    local.get 15
    local.get 11
    f32.sub
    local.set 11
    local.get 8
    local.get 13
    f32.sub
    local.set 13
    i32.const 3434688
    i32.load
    i32.const 871
    call_indirect (type 0)
    local.get 9
    local.get 9
    f32.mul
    local.get 11
    local.get 11
    f32.mul
    f32.add
    local.get 13
    local.get 13
    f32.mul
    f32.add
    local.set 9
    i32.const 3432484
    i32.load
    i32.const 871
    call_indirect (type 0)
    block  ;; label = @1
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=148
      if  ;; label = @2
        i32.const 3432484
        i32.load
        i32.const 871
        call_indirect (type 0)
        i32.const 3432484
        i32.load
        i32.load offset=92
        i32.load offset=148
        i32.load offset=12
        br_if 1 (;@1;)
      end
      i32.const 3427900
      i32.load
      i32.const 64
      i32.const 888
      call_indirect (type 2)
      local.set 0
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3432484
      i32.load
      i32.load offset=92
      local.get 0
      i32.store offset=148
      i32.const 3432484
      i32.load
      i32.load offset=92
      drop
    end
    i32.const 3432484
    i32.load
    i32.const 871
    call_indirect (type 0)
    i32.const 3468552
    i32.load
    drop
    i32.const 3432484
    i32.load
    i32.load offset=92
    i32.load offset=132
    i32.load offset=12
    local.set 1
    local.get 16
    local.get 10
    f32.sub
    local.set 16
    local.get 15
    local.get 12
    f32.sub
    local.set 15
    local.get 8
    local.get 14
    f32.sub
    local.set 11
    local.get 2
    i32.load offset=212
    local.set 3
    local.get 2
    i32.load offset=216
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=176
    local.get 2
    local.get 4
    i32.store offset=180
    local.get 2
    local.get 2
    i32.load offset=220
    i32.store offset=184
    local.get 2
    f32.load offset=176
    f32.const 0x1.ccccccp-1 (;=0.9;)
    f32.mul
    local.set 13
    local.get 2
    f32.load offset=180
    f32.const 0x1.ccccccp-1 (;=0.9;)
    f32.mul
    local.set 17
    local.get 2
    f32.load offset=184
    f32.const 0x1.ccccccp-1 (;=0.9;)
    f32.mul
    local.set 18
    i32.const 4523251
    i32.load8_u
    i32.eqz
    if  ;; label = @1
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
    local.get 10
    local.get 10
    f32.mul
    local.get 12
    local.get 12
    f32.mul
    f32.add
    local.get 14
    local.get 14
    f32.mul
    f32.add
    f32.sqrt
    local.set 8
    block  ;; label = @1
      local.get 8
      f32.const 0x1.4f8b58p-17 (;=1e-05;)
      f32.gt
      if  ;; label = @2
        local.get 14
        local.get 8
        f32.div
        local.set 14
        local.get 12
        local.get 8
        f32.div
        local.set 12
        local.get 10
        local.get 8
        f32.div
        local.set 10
        br 1 (;@1;)
      end
      i32.const 4525920
      i32.load8_u
      i32.eqz
      if  ;; label = @2
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
      local.set 0
      local.get 0
      f32.load offset=8
      local.set 14
      local.get 0
      f32.load offset=4
      local.set 12
      local.get 0
      f32.load
      local.set 10
    end
    i32.const 3432484
    i32.load
    i32.load offset=92
    i32.load offset=148
    local.set 5
    i32.const 4525921
    i32.load8_u
    i32.eqz
    if  ;; label = @1
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
    local.set 0
    local.get 2
    local.get 11
    f32.store offset=144
    local.get 2
    local.get 18
    f32.store offset=128
    local.get 2
    local.get 14
    f32.store offset=112
    local.get 2
    local.get 12
    f32.store offset=108
    local.get 2
    local.get 10
    f32.store offset=104
    local.get 2
    local.get 17
    f32.store offset=124
    local.get 2
    local.get 13
    f32.store offset=120
    local.get 2
    local.get 15
    f32.store offset=140
    local.get 2
    local.get 16
    f32.store offset=136
    local.get 0
    i32.load
    local.set 4
    local.get 0
    i32.load offset=4
    local.set 3
    local.get 4
    local.set 6
    local.get 3
    local.set 7
    local.get 0
    i32.load offset=8
    local.set 3
    local.get 0
    i32.load offset=12
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=96
    local.get 2
    local.get 4
    i32.store offset=100
    local.get 2
    local.get 2
    i32.load offset=144
    i32.store offset=80
    local.get 2
    i32.const -64
    i32.sub
    local.get 2
    i32.load offset=128
    i32.store
    local.get 2
    local.get 2
    i32.load offset=112
    i32.store offset=48
    local.get 2
    i32.load offset=136
    local.set 4
    local.get 2
    i32.load offset=140
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=72
    local.get 2
    local.get 3
    i32.store offset=76
    local.get 2
    i32.load offset=120
    local.set 3
    local.get 2
    i32.load offset=124
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=56
    local.get 2
    local.get 4
    i32.store offset=60
    local.get 2
    i32.load offset=104
    local.set 4
    local.get 2
    i32.load offset=108
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=40
    local.get 2
    local.get 3
    i32.store offset=44
    local.get 7
    local.set 3
    local.get 2
    local.get 6
    i32.store offset=88
    local.get 2
    local.get 3
    i32.store offset=92
    local.get 2
    i32.load offset=96
    local.set 3
    local.get 2
    i32.load offset=100
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=32
    local.get 2
    local.get 4
    i32.store offset=36
    local.get 2
    i32.load offset=88
    local.set 4
    local.get 2
    i32.load offset=92
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=24
    local.get 2
    local.get 3
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
    local.get 5
    local.get 2
    i32.const 24
    i32.add
    local.get 9
    f32.sqrt
    i32.const 0
    call $j36318 ;; 
    local.set 3
    i32.const 0
    local.set 0
    loop  ;; label = @1
      block  ;; label = @2
        i32.const 0
        local.set 5
        local.get 0
        local.get 3
        i32.lt_s
        local.set 4
        local.get 4
        i32.eqz
        br_if 0 (;@2;)
        loop  ;; label = @3
          local.get 1
          local.get 5
          i32.gt_s
          if  ;; label = @4
            i32.const 3432484
            i32.load
            i32.const 871
            call_indirect (type 0)
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=148
            local.get 0
            i32.const 44
            i32.mul
            i32.add
            i32.const 16
            i32.add
            i32.const 0
            i32.const 2790
            call_indirect (type 2)
            i32.const 0
            i32.const 944
            call_indirect (type 2)
            local.set 7
            i32.const 3432484
            i32.load
            i32.load offset=92
            i32.load offset=132
            local.get 5
            i32.const 3468556
            i32.load
            i32.const 895
            call_indirect (type 5)
            i32.const 0
            i32.const 944
            call_indirect (type 2)
            local.set 6
            i32.const 3435416
            i32.load
            i32.const 871
            call_indirect (type 0)
            local.get 7
            local.get 6
            i32.const 0
            i32.const 924
            call_indirect (type 5)
            br_if 2 (;@2;)
            local.get 5
            i32.const 1
            i32.add
            local.set 5
            br 1 (;@3;)
          end
        end
        local.get 0
        i32.const 1
        i32.add
        local.set 0
        br 1 (;@1;)
      end
    end
    local.get 2
    i32.const 224
    i32.add
    global.set 0
    local.get 4
    return)
