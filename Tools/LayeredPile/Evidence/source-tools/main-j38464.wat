  (func $j38464 (type 0) (param i32 i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 0
    local.get 0
    global.set 0
    i32.const 4507802
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430028
      call $j870
      i32.const 3457592
      call $j870
      i32.const 3457628
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3434332
      call $j870
      i32.const 3435224
      call $j870
      i32.const 3438836
      call $j870
      i32.const 4507802
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871
    i32.const 0
    call $j1923
    i32.eqz
    if  ;; label = @1
      i32.const 3434332
      i32.load
      call $j871
      i32.const 3434332
      i32.load
      i32.load offset=92
      i32.load offset=4
      i32.const 3457628
      i32.load
      call $j1167
      i32.const 3434332
      i32.load
      i32.load offset=92
      i32.load offset=8
      i32.const 3457628
      i32.load
      call $j1167
      i32.const 3434332
      i32.load
      i32.load offset=92
      i32.load offset=12
      i32.const 3457592
      i32.load
      call $j1167
      i32.const 3435224
      i32.load
      call $j871
      i32.const 4514392
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3435224
        call $j870
        i32.const 4514392
        i32.const 1
        i32.store8
      end
      i32.const 3435224
      i32.load
      call $j871
      i32.const 3435224
      i32.load
      i32.load offset=92
      drop
      loop  ;; label = @2
        block  ;; label = @3
          i32.const 3434332
          i32.load
          call $j871
          local.get 2
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          i32.load offset=12
          i32.ge_s
          br_if 0 (;@3;)
          i32.const 3434332
          i32.load
          call $j871
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          local.get 2
          i32.const 2
          i32.shl
          i32.add
          i32.load offset=16
          local.set 1
          i32.const 3438836
          i32.load
          local.set 4
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          local.get 2
          i32.const 2
          i32.shl
          i32.add
          i32.const 16
          i32.add
          local.set 5
          local.get 0
          i32.const 16
          i32.add
          local.set 3
          local.get 3
          i32.const -1
          i32.store offset=4
          local.get 3
          local.get 4
          i32.store
          local.get 3
          local.get 5
          i32.load
          i32.store offset=8
          local.get 0
          i32.const 16
          i32.add
          i32.const 0
          call $j956
          local.set 3
          i32.const 3435224
          i32.load
          call $j871
          local.get 3
          i32.const 0
          call $j3052
          local.set 3
          i32.const 3438836
          i32.load
          local.set 4
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=16
          local.get 2
          i32.const 2
          i32.shl
          i32.add
          i32.const 16
          i32.add
          local.set 5
          local.get 0
          i32.const -1
          i32.store offset=4
          local.get 0
          local.get 4
          i32.store
          local.get 0
          local.get 5
          i32.load
          i32.store offset=8
          local.get 0
          i32.const 0
          call $j956
          local.set 4
          i32.const 3430028
          i32.load
          call $j871
          local.get 4
          i32.const 0
          call $j4354
          drop
          local.get 2
          local.get 1
          local.get 3
          local.get 2
          local.get 2
          call $j13216
          local.get 2
          i32.const 1
          i32.add
          local.set 2
          br 1 (;@2;)
        end
      end
      local.get 2
      i32.const 3
      i32.const 0
      local.get 2
      local.get 2
      call $j13216
      local.get 2
      i32.const 4
      i32.const 0
      local.get 2
      local.get 2
      call $j13216
    end
    local.get 2
    local.get 2
    call $j3725
    local.get 0
    i32.const 32
    i32.add
    global.set 0)
