  (func $j6737 (type 8) (param i32 i32 i32 i32 i32)
    (local i32)
    i32.const 4505050
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3427732
      call $j870 ;; 
      i32.const 3495724
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 4505050
      i32.const 1
      i32.store8
    end
    i32.const 3435416
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      local.get 3
      i32.const 0
      i32.const 0
      call $j924 ;; 
      br_if 0 (;@1;)
      local.get 3
      i32.const 0
      call $j1782 ;; 
      local.set 4
      local.get 2
      i32.load
      local.set 0
      block  ;; label = @2
        local.get 0
        if  ;; label = @3
          local.get 0
          i32.load offset=12
          local.get 4
          i32.ge_s
          br_if 1 (;@2;)
        end
        local.get 0
        local.get 0
        local.get 0
        call $j5909 ;; HighlightEffect.DestroyMaterialArray
        local.get 2
        i32.const 3427732
        i32.load
        local.get 4
        call $j888 ;; 
        i32.store
      end
      i32.const 0
      local.set 3
      loop  ;; label = @2
        local.get 3
        local.get 4
        i32.ge_s
        br_if 1 (;@1;)
        local.get 2
        i32.load
        local.get 3
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.set 0
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 0
        i32.const 0
        i32.const 0
        call $j924 ;; 
        if  ;; label = @3
          local.get 2
          i32.load
          local.set 0
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 1
          i32.const 3495724
          i32.load
          call $j1764 ;; 
          local.set 5
          local.get 0
          local.get 5
          call $j877 ;; 
          local.get 0
          local.get 3
          i32.const 2
          i32.shl
          i32.add
          local.get 5
          i32.store offset=16
        end
        local.get 3
        i32.const 1
        i32.add
        local.set 3
        br 0 (;@2;)
      end
      unreachable
    end)
