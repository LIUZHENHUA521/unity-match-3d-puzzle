  (func $j20233 (type 65) (param i32 i32 f32 f32 i32)
    (local i32)
    local.get 0
    local.get 2
    f32.store offset=908
    local.get 0
    local.get 3
    f32.store offset=900
    local.get 1
    local.set 5
    local.get 5
    i32.load
    local.set 1
    local.get 5
    i32.load offset=4
    local.set 4
    local.get 0
    local.get 1
    i32.store offset=912
    local.get 0
    local.get 4
    i32.store offset=916
    local.get 5
    i32.load offset=8
    local.set 4
    local.get 5
    i32.load offset=12
    local.set 1
    local.get 0
    local.get 4
    i32.store offset=920
    local.get 0
    local.get 1
    i32.store offset=924
    i32.const 0
    i32.const 1519
    call_indirect (type 21)
    local.set 2
    local.get 0
    i32.const 1
    i32.store8 offset=928
    local.get 0
    local.get 2
    f32.store offset=904
    local.get 0
    f32.load offset=76
    f32.const 0x0p+0 (;=0;)
    f32.eq
    if  ;; label = @1
      local.get 0
      i32.const 0
      local.get 0
      call $j5908 ;; HighlightEffect.UpdateMaterialProperties
    end)
