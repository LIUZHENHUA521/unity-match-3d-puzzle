  (func $j19876 (type 8) (param i32 i32 i32 i32 i32)
    (local i32 i32 f32 f32 f32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    i32.const 4509399
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3457648
      call $j870
      i32.const 3457612
      call $j870
      i32.const 3457616
      call $j870
      i32.const 3491080
      call $j870
      i32.const 3435480
      call $j870
      i32.const 3533228
      call $j870
      i32.const 4509399
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 0
      i32.load offset=712
      i32.const 1
      i32.const 3457648
      i32.load
      call $j1065
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=716
      i32.const 1
      i32.const 3457612
      i32.load
      call $j1065
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=716
      i32.const 1
      i32.const 3457616
      i32.load
      call $j1060
      local.set 5
      i32.const 3435480
      i32.load
      call $j871
      local.get 5
      i32.const 0
      i32.const 0
      call $j5812
      i32.const 3491080
      i32.load
      call $j1083
      local.set 5
      local.get 1
      f32.load offset=8
      local.set 7
      local.get 1
      f32.load offset=4
      local.set 8
      local.get 1
      f32.load
      local.set 9
      i32.const 4524774
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        call $j870
        i32.const 4524774
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 1
      local.get 1
      f32.load offset=80
      local.set 10
      local.get 9
      local.get 1
      f32.load offset=72
      f32.const 0x1p-1 (;=0.5;)
      f32.mul
      f32.add
      local.set 9
      local.get 8
      local.get 1
      f32.load offset=76
      f32.const 0x1p-1 (;=0.5;)
      f32.mul
      f32.add
      local.set 8
      local.get 7
      local.get 10
      f32.const 0x1p-1 (;=0.5;)
      f32.mul
      f32.add
      local.set 7
      local.get 2
      i32.const 0
      call $j925
      local.set 1
      local.get 0
      f32.load offset=584
      local.set 10
      local.get 4
      local.get 7
      f32.store offset=24
      local.get 4
      local.get 4
      i32.load offset=24
      i32.store offset=8
      local.get 4
      local.get 8
      f32.store offset=20
      local.get 4
      local.get 9
      f32.store offset=16
      local.get 4
      i32.load offset=20
      local.set 6
      local.get 4
      local.get 4
      i32.load offset=16
      i32.store
      local.get 4
      local.get 6
      i32.store offset=4
      local.get 5
      local.get 4
      local.get 1
      local.get 10
      i32.const 0
      call $j31485
    end
    local.get 0
    i32.load offset=676
    local.set 1
    local.get 1
    i32.load offset=12
    local.set 5
    local.get 5
    if  ;; label = @1
      local.get 1
      local.get 3
      local.get 5
      call $i32s_rem
      i32.const 2
      i32.shl
      i32.add
      i32.load offset=16
      local.get 2
      i32.const 0
      call $j944
      i32.const 0
      call $j8126
      local.get 0
      i32.load offset=676
      local.set 1
      local.get 1
      local.set 2
      local.get 2
      local.get 3
      local.get 1
      i32.load offset=12
      call $i32s_rem
      i32.const 2
      i32.shl
      i32.add
      i32.load offset=16
      i32.const 1
      i32.const 0
      call $j2810
      local.get 0
      i32.const 3533228
      i32.load
      i32.const 0
      call $j1446
      local.get 0
      i32.const 3533228
      i32.load
      i32.const 0
      call $j1535
      drop
    end
    local.get 4
    i32.const 32
    i32.add
    global.set 0)
