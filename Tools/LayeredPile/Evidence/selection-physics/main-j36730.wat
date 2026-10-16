  (func $j36730 (type 1) (param i32 i32 i32)
    (local i32)
    global.get 0
    i32.const 16
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
      local.get 2
      local.get 1
      i32.load offset=8
      i32.store offset=8
      local.get 1
      i32.load offset=4
      local.set 3
      local.get 2
      local.get 1
      i32.load
      i32.store
      local.get 2
      local.get 3
      i32.store offset=4
      local.get 0
      local.get 2
      i32.const 0
      call $j18670 ;; Rigidbody.AddForce
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0)
