  (func $j19261 (type 16) (param i32 f32 i32)
    (local i32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    local.get 0
    i32.load offset=20
    i32.const 0
    call $j7845 ;; Rigidbody.get_isKinematic
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.load offset=20
      local.set 0
      f32.const -0x1.4p+3 (;=-10;)
      f32.const 0x1.4p+3 (;=10;)
      i32.const 0
      call $j1345 ;; Random.Range
      local.set 4
      local.get 2
      f32.const -0x1.4p+3 (;=-10;)
      f32.const 0x1.4p+3 (;=10;)
      i32.const 0
      call $j1345 ;; Random.Range
      f32.store offset=24
      local.get 2
      local.get 2
      i32.load offset=24
      i32.store offset=8
      local.get 2
      local.get 1
      f32.store offset=20
      local.get 2
      local.get 4
      f32.store offset=16
      local.get 2
      i32.load offset=20
      local.set 3
      local.get 2
      local.get 2
      i32.load offset=16
      i32.store
      local.get 2
      local.get 3
      i32.store offset=4
      local.get 0
      local.get 2
      i32.const 0
      call $j6538 ;; Rigidbody.set_velocity
    end
    local.get 2
    i32.const 32
    i32.add
    global.set 0)
