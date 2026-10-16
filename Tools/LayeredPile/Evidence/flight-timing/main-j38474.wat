  (func $j38474 (type 44) (param i32 i32 i32 i32) (result f32)
    (local i32 i32 i32 f32 f32 f32)
    global.get 0
    i32.const 80
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4507784
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434152
      call $j870 ;; 
      i32.const 4507784
      i32.const 1
      i32.store8
    end
    i32.const 3434152
    i32.load
    call $j871 ;; 
    local.get 3
    i32.const -64
    i32.sub
    local.get 1
    i32.load offset=8
    i32.store
    local.get 1
    local.set 4
    local.get 4
    i32.load
    local.set 5
    local.get 4
    i32.load offset=4
    local.set 6
    local.get 3
    local.get 5
    i32.store offset=56
    local.get 3
    local.get 6
    i32.store offset=60
    local.get 3
    i32.const 72
    i32.add
    local.get 3
    i32.const 56
    i32.add
    i32.const 0
    call $j6689 ;; KUIManager.WorldToUIPosition
    local.get 3
    f32.load offset=76
    local.set 7
    local.get 3
    local.get 2
    i32.load offset=8
    i32.store offset=48
    local.get 2
    local.set 4
    local.get 4
    i32.load
    local.set 6
    local.get 4
    i32.load offset=4
    local.set 5
    local.get 3
    local.get 6
    i32.store offset=40
    local.get 3
    local.get 5
    i32.store offset=44
    local.get 3
    i32.const 72
    i32.add
    local.get 3
    i32.const 40
    i32.add
    i32.const 0
    call $j6689 ;; KUIManager.WorldToUIPosition
    local.get 7
    local.get 3
    f32.load offset=76
    f32.sub
    local.set 7
    local.get 0
    i32.load offset=56
    f32.load offset=48
    local.set 8
    local.get 3
    local.get 1
    i32.load offset=8
    i32.store offset=32
    local.get 1
    local.set 4
    local.get 4
    i32.load
    local.set 5
    local.get 4
    i32.load offset=4
    local.set 6
    local.get 3
    local.get 5
    i32.store offset=24
    local.get 3
    local.get 6
    i32.store offset=28
    local.get 3
    i32.const 72
    i32.add
    local.get 3
    i32.const 24
    i32.add
    i32.const 0
    call $j6689 ;; KUIManager.WorldToUIPosition
    local.get 3
    f32.load offset=72
    local.set 9
    local.get 3
    local.get 2
    i32.load offset=8
    i32.store offset=16
    local.get 2
    local.set 4
    local.get 4
    i32.load
    local.set 6
    local.get 4
    i32.load offset=4
    local.set 5
    local.get 3
    local.get 6
    i32.store offset=8
    local.get 3
    local.get 5
    i32.store offset=12
    local.get 3
    i32.const 72
    i32.add
    local.get 3
    i32.const 8
    i32.add
    i32.const 0
    call $j6689 ;; KUIManager.WorldToUIPosition
    local.get 7
    f32.abs
    local.get 8
    f32.mul
    local.get 9
    local.get 3
    f32.load offset=72
    f32.sub
    f32.abs
    local.get 0
    i32.load offset=56
    f32.load offset=52
    f32.mul
    f32.add
    local.set 7
    local.get 3
    i32.const 80
    i32.add
    global.set 0
    local.get 7
    return)
