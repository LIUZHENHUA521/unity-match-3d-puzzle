  (func $j19264 (type 0) (param i32 i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const -64
    i32.add
    local.set 1
    local.get 1
    global.set 0
    local.get 0
    i32.load offset=20
    local.set 5
    i32.const 4525920
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3439908
      call $j870 ;; 
      i32.const 4525920
      i32.const 1
      i32.store8
    end
    i32.const 3439908
    i32.load
    i32.load offset=92
    local.set 2
    local.get 2
    i32.load
    local.set 3
    local.get 2
    i32.load offset=4
    local.set 4
    local.get 1
    local.get 2
    i32.load offset=8
    i32.store offset=56
    local.get 1
    local.get 1
    i32.load offset=56
    i32.store offset=24
    local.get 1
    local.get 3
    i32.store offset=48
    local.get 1
    local.get 4
    i32.store offset=52
    local.get 1
    i32.load offset=48
    local.set 4
    local.get 1
    i32.load offset=52
    local.set 3
    local.get 1
    local.get 4
    i32.store offset=16
    local.get 1
    local.get 3
    i32.store offset=20
    local.get 5
    local.get 1
    i32.const 16
    i32.add
    i32.const 0
    call $j6538 ;; Rigidbody.set_velocity
    local.get 0
    i32.load offset=20
    local.set 0
    i32.const 4525920
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3439908
      call $j870 ;; 
      i32.const 4525920
      i32.const 1
      i32.store8
    end
    i32.const 3439908
    i32.load
    i32.load offset=92
    local.set 5
    local.get 5
    local.set 2
    local.get 2
    i32.load
    local.set 3
    local.get 2
    i32.load offset=4
    local.set 4
    local.get 1
    local.get 2
    i32.load offset=8
    i32.store offset=40
    local.get 1
    local.get 1
    i32.load offset=40
    i32.store offset=8
    local.get 1
    local.get 3
    i32.store offset=32
    local.get 1
    local.get 4
    i32.store offset=36
    local.get 1
    i32.load offset=32
    local.set 4
    local.get 1
    i32.load offset=36
    local.set 3
    local.get 1
    local.get 4
    i32.store
    local.get 1
    local.get 3
    i32.store offset=4
    local.get 0
    local.get 1
    i32.const 0
    call $j18665 ;; Rigidbody.set_angularVelocity
    local.get 1
    i32.const -64
    i32.sub
    global.set 0)
