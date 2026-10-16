  (func $j63999 (type 0) (param i32 i32)
    (local i32 f32 f32)
    i32.const 4507841
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870 ;; 
      i32.const 3438972
      call $j870 ;; 
      i32.const 3502300
      call $j870 ;; 
      i32.const 3502396
      call $j870 ;; 
      i32.const 3502500
      call $j870 ;; 
      i32.const 3515512
      call $j870 ;; 
      i32.const 4507841
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=8
    i32.const 0
    call $j944 ;; Component.get_transform
    local.set 1
    local.get 0
    i32.load offset=8
    f32.load offset=64
    local.set 3
    local.get 0
    f32.load offset=12
    local.set 4
    i32.const 3434312
    i32.load
    call $j871 ;; 
    local.get 1
    local.get 3
    local.get 4
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load
    f32.load offset=612
    f32.sub
    f32.const 0x0p+0 (;=0;)
    f32.max
    i32.const 0
    call $j1727 ;; ShortcutExtensions.DOScale
    i32.const 1
    i32.const 3502500
    i32.load
    call $j1281 ;; 
    local.set 2
    local.get 0
    i32.load offset=20
    local.set 1
    local.get 1
    i32.eqz
    if  ;; label = @1
      i32.const 3438972
      i32.load
      call $j873 ;; 
      local.set 1
      local.get 1
      local.get 0
      i32.const 3515512
      i32.load
      i32.const 0
      call $j1486 ;; TweenCallback..ctor
      local.get 0
      local.get 1
      i32.store offset=20
    end
    local.get 2
    local.get 1
    i32.const 3502300
    i32.load
    call $j1889 ;; 
    local.set 0
    i32.const 3434312
    i32.load
    call $j871 ;; 
    local.get 0
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load
    i32.load offset=608
    i32.const 3502396
    i32.load
    call $j1183 ;; 
    drop)
