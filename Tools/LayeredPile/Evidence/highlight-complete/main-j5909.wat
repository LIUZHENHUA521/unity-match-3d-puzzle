  (func $j5909 (type 1) (param i32 i32 i32)
    block  ;; label = @1
      local.get 1
      i32.eqz
      br_if 0 (;@1;)
      i32.const 0
      local.set 0
      loop  ;; label = @2
        local.get 0
        local.get 1
        i32.load offset=12
        i32.ge_s
        br_if 1 (;@1;)
        local.get 0
        local.get 1
        local.get 0
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.get 0
        call $j5240 ;; HighlightEffect.DestroyMaterial
        local.get 0
        i32.const 1
        i32.add
        local.set 0
        br 0 (;@2;)
      end
      unreachable
    end)
