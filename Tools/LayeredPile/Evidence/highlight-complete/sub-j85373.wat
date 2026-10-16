  (func $j85373 (type 4) (param i32 i32)
    local.get 0
    i32.const 0
    i32.const 1519
    call_indirect (type 21)
    f32.store offset=432
    local.get 0
    i32.load8_u offset=244
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.const 1
      i32.store8 offset=244
      local.get 0
      i32.const 0
      local.get 0
      call $j5908 ;; HighlightEffect.UpdateMaterialProperties
    end)
