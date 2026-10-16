  (func $j36803 (type 0) (param i32 i32)
    (local i32 i32 i32 f32 f32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507820
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870 ;; 
      i32.const 4507820
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 2
    i32.const 3435416
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      local.get 2
      i32.const 0
      i32.const 0
      call $j924 ;; Object.op_Equality
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=12
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=16
      i32.const 0
      i32.const 0
      call $j1360 ;; ShortcutExtensions.DOKill
      drop
      local.get 0
      i32.load offset=16
      local.set 2
      local.get 1
      local.get 0
      i32.load offset=28
      i32.store offset=24
      local.get 0
      i32.load offset=20
      local.set 3
      local.get 0
      i32.load offset=24
      local.set 4
      local.get 1
      local.get 3
      i32.store offset=16
      local.get 1
      local.get 4
      i32.store offset=20
      local.get 2
      local.get 1
      i32.const 16
      i32.add
      i32.const 0
      call $j1161 ;; Transform.set_localScale
      local.get 0
      i32.load offset=16
      local.set 2
      i32.const 4518492
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        call $j870 ;; 
        i32.const 4518492
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 0
      local.get 0
      f32.load offset=12
      f32.const 0x1.99999ap-5 (;=0.05;)
      f32.mul
      local.set 5
      local.get 0
      f32.load offset=16
      f32.const 0x1.99999ap-5 (;=0.05;)
      f32.mul
      local.set 6
      local.get 1
      local.get 0
      f32.load offset=20
      f32.const 0x1.99999ap-5 (;=0.05;)
      f32.mul
      f32.store offset=40
      local.get 1
      local.get 1
      i32.load offset=40
      i32.store offset=8
      local.get 1
      local.get 6
      f32.store offset=36
      local.get 1
      local.get 5
      f32.store offset=32
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
      local.get 2
      local.get 1
      f32.const 0x1p-1 (;=0.5;)
      i32.const 10
      f32.const 0x1p+0 (;=1;)
      i32.const 0
      call $j2901 ;; ShortcutExtensions.DOPunchScale
      drop
    end
    local.get 1
    i32.const 48
    i32.add
    global.set 0)
