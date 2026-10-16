  (func $j8126 (type 1) (param i32 i32 i32)
    i32.const 4505039
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870 ;; 
      i32.const 4505039
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=424
    local.set 2
    i32.const 3435416
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      local.get 1
      local.get 2
      i32.const 0
      call $j924 ;; 
      br_if 0 (;@1;)
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 1
      i32.const 0
      i32.const 0
      call $j924 ;; 
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=57
      if  ;; label = @2
        local.get 0
        i32.const 0
        local.get 0
        call $j2810 ;; HighlightEffect.SetHighlighted
      end
      local.get 0
      local.get 1
      i32.store offset=424
      local.get 0
      local.get 0
      call $j8125 ;; HighlightEffect.SetupMaterial
    end)
