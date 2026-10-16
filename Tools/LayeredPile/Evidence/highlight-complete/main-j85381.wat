  (func $j85381 (type 0) (param i32 i32)
    local.get 0
    i32.load8_u offset=749
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.const 0
      call $j14065 ;; 
      local.get 0
      call $j20234 ;; HighlightEffect.DoOnRenderObject
    end)
