  (func $j38494 (type 1) (param i32 i32 i32)
    (local i32)
    i32.const 4509416
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870 ;; 
      i32.const 4509416
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=465
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=820
      i32.eqz
      br_if 0 (;@1;)
      block  ;; label = @2
        local.get 1
        i32.eqz
        if  ;; label = @3
          local.get 0
          local.get 0
          call $j19870 ;; LevelManager.GetNextSameObj
          local.set 2
          local.get 0
          i32.load offset=824
          local.set 3
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 3
          i32.const 0
          call $j982 ;; 
          i32.eqz
          br_if 2 (;@1;)
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 2
          i32.const 0
          call $j982 ;; 
          i32.eqz
          br_if 2 (;@1;)
          local.get 0
          i32.const 824
          i32.add
          local.set 0
          br 1 (;@2;)
        end
        i32.const 0
        local.set 2
        local.get 0
        i32.const 0
        i32.store8 offset=820
        local.get 0
        i32.const 824
        i32.add
        local.set 0
      end
      local.get 0
      i32.load
      local.get 2
      i32.const 0
      call $j8126 ;; HighlightEffect.SetTarget
      local.get 0
      i32.load
      local.get 1
      i32.const 1
      i32.xor
      i32.const 0
      call $j2810 ;; HighlightEffect.SetHighlighted
    end)
