  (func $j31214 (type 5) (param i32 i32 i32) (result i32)
    (local f64)
    local.get 2
    call $j928 ;; 
    block  ;; label = @1
      local.get 0
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=144
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      local.get 1
      i32.store offset=116
      local.get 1
      i32.const 32
      i32.sub
      i32.const 4
      i32.lt_u
      if  ;; label = @2
        local.get 0
        local.set 1
        local.get 0
        f32.load offset=124
        f64.promote_f32
        local.set 3
        block  ;; label = @3
          local.get 3
          f64.abs
          f64.const 0x1p+31 (;=2.14748e+09;)
          f64.lt
          if  ;; label = @4
            local.get 3
            call $asm2wasm_f64_to_int
            local.set 2
            br 1 (;@3;)
          end
          i32.const -2147483648
          local.set 2
        end
        local.get 1
        local.get 2
        f32.convert_i32_s
        f32.store offset=124
      end
      local.get 0
      i32.const 0
      i32.store offset=120
    end
    local.get 0
    return)
