  (func $j63997 (type 0) (param i32 i32)
    (local i32 i32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507842
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870 ;; 
      i32.const 4507842
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=8
    i32.const 0
    call $j944 ;; Component.get_transform
    local.set 3
    i32.const 4518492
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3439908
      call $j870 ;; 
      i32.const 4518492
      i32.const 1
      i32.store8
    end
    i32.const 3439908
    i32.load
    i32.load offset=92
    local.set 2
    local.get 2
    f32.load offset=20
    local.set 6
    local.get 2
    f32.load offset=16
    local.set 7
    local.get 2
    f32.load offset=12
    local.set 4
    i32.const 3434312
    i32.load
    call $j871 ;; 
    local.get 4
    local.set 8
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load
    f32.load offset=620
    local.set 4
    local.get 0
    f32.load offset=16
    local.set 5
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load
    f32.load offset=624
    local.set 9
    local.get 1
    local.get 6
    local.get 4
    f32.mul
    local.get 5
    f32.mul
    f32.store offset=24
    local.get 1
    local.get 1
    i32.load offset=24
    i32.store offset=8
    local.get 1
    local.get 7
    local.get 4
    f32.mul
    local.get 5
    f32.mul
    f32.store offset=20
    local.get 1
    local.get 8
    local.get 4
    f32.mul
    local.get 5
    f32.mul
    f32.store offset=16
    local.get 1
    i32.load offset=20
    local.set 2
    local.get 1
    local.get 1
    i32.load offset=16
    i32.store
    local.get 1
    local.get 2
    i32.store offset=4
    local.get 3
    local.get 1
    local.get 9
    i32.const 1
    f32.const 0x1p+0 (;=1;)
    i32.const 0
    call $j2901 ;; ShortcutExtensions.DOPunchScale
    drop
    local.get 0
    i32.load offset=8
    i32.const 1
    i32.store8 offset=93
    local.get 1
    i32.const 32
    i32.add
    global.set 0)
