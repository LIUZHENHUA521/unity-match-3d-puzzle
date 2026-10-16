  (func $j92873 (type 0) (param i32 i32)
    (local i32 i32 i32 f32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4508850
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3429624
      call $j870
      i32.const 3434312
      call $j870
      i32.const 4508850
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=24
    i32.const 0
    call $j6027
    local.set 5
    local.get 0
    i32.load offset=24
    local.get 5
    i32.const 3429624
    i32.load
    i32.load offset=92
    f32.load offset=4
    f32.mul
    i32.const 0
    call $j3793
    local.get 0
    i32.load offset=24
    i32.const 3429624
    i32.load
    i32.load offset=92
    f32.load offset=4
    f32.const 0x1.ep+5 (;=60;)
    f32.mul
    i32.const 0
    call $j10377
    i32.const 3434312
    i32.load
    call $j871
    i32.const 1
    local.set 2
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load offset=8
            br_table 1 (;@3;) 2 (;@2;) 0 (;@4;) 3 (;@1;)
          end
          local.get 1
          i32.const 32
          i32.add
          local.get 0
          i32.load offset=32
          i32.const 0
          call $j1340
          local.get 0
          i32.load offset=32
          local.set 2
          local.get 1
          f32.load offset=36
          i32.const 3429624
          i32.load
          i32.load offset=92
          f32.load offset=4
          f32.mul
          local.set 5
          local.get 1
          local.get 1
          f32.load offset=40
          f32.store offset=24
          local.get 1
          local.get 1
          i32.load offset=24
          i32.store offset=8
          local.get 1
          local.get 5
          f32.store offset=20
          local.get 1
          i32.const 0
          i32.store offset=16
          local.get 1
          i32.load offset=20
          local.set 3
          local.get 1
          local.get 1
          i32.load offset=16
          i32.store
          local.get 1
          local.get 3
          i32.store offset=4
          local.get 2
          local.get 1
          i32.const 0
          call $j1271
          i32.const 0
          local.set 2
          br 1 (;@2;)
        end
        i32.const 1
        local.set 4
      end
      local.get 0
      local.get 2
      i32.store8 offset=45
      local.get 0
      local.get 4
      i32.store8 offset=44
    end
    local.get 1
    i32.const 48
    i32.add
    global.set 0)
