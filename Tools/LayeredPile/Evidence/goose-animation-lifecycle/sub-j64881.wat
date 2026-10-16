  (func $j64881 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 f32 f32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507844
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3440336
      i32.const 870
      call_indirect (type 0)
      i32.const 4507844
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          local.get 0
          i32.load offset=8
          br_table 0 (;@3;) 1 (;@2;) 2 (;@1;)
        end
        local.get 0
        i32.const -1
        i32.store offset=8
        i32.const 3440336
        i32.load
        i32.const 873
        call_indirect (type 1)
        local.set 2
        local.get 2
        f32.const 0x1.333334p-2 (;=0.3;)
        f32.store offset=8
        local.get 0
        local.get 2
        i32.store offset=12
        i32.const 1
        local.set 4
        local.get 0
        i32.const 1
        i32.store offset=8
        br 1 (;@1;)
      end
      local.get 0
      i32.load offset=16
      local.set 2
      local.get 0
      i32.const -1
      i32.store offset=8
      local.get 2
      i32.const 0
      i32.store8 offset=19
      local.get 2
      i32.const 1
      i32.store8 offset=16
      local.get 2
      i32.load offset=24
      i32.const 1
      i32.const 0
      i32.const 907
      call_indirect (type 3)
      local.get 2
      i32.load offset=28
      i32.const 1
      i32.const 0
      i32.const 907
      call_indirect (type 3)
      local.get 2
      i32.const 1
      local.get 0
      i32.const 7954
      call_indirect (type 3)
      local.get 2
      i32.load offset=20
      local.set 5
      local.get 0
      f32.load offset=32
      local.set 6
      local.get 0
      f32.load offset=20
      local.get 6
      f32.mul
      local.set 7
      local.get 0
      f32.load offset=24
      local.get 6
      f32.mul
      local.set 8
      local.get 1
      i32.const 24
      i32.add
      local.set 3
      local.get 3
      local.get 0
      f32.load offset=28
      local.get 6
      f32.mul
      f32.store
      local.get 1
      local.get 3
      i32.load
      i32.store offset=8
      local.get 1
      local.get 8
      f32.store offset=20
      local.get 1
      local.get 7
      f32.store offset=16
      local.get 1
      i32.load offset=20
      local.set 0
      local.get 1
      local.get 1
      i32.load offset=16
      i32.store
      local.get 1
      local.get 0
      i32.store offset=4
      local.get 5
      local.get 1
      i32.const 0
      i32.const 6538
      call_indirect (type 3)
      local.get 2
      i32.const 1
      local.get 3
      i32.const 7955
      call_indirect (type 3)
    end
    local.get 1
    i32.const 32
    i32.add
    global.set 0
    local.get 4
    return)