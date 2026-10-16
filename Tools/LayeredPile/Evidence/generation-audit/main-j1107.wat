  (func $j1107 (type 83) (param f64) (result f64)
    (local i32 i32 i32 i32 i32 f64 f64 f64)
    global.get 0
    i32.const 16
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    local.get 0
    local.get 4
    i32.const 8
    i32.add
    call $j5462
    local.set 6
    block  ;; label = @1
      local.get 0
      f64.const 0x0p+0 (;=0;)
      f64.ge
      if  ;; label = @2
        local.get 6
        f64.const 0x1p-1 (;=0.5;)
        f64.eq
        if  ;; label = @3
          local.get 4
          f64.load offset=8
          local.set 0
          local.get 0
          f64.const 0x1p+0 (;=1;)
          f64.add
          local.set 6
          local.get 0
          local.set 7
          local.get 6
          local.set 8
          block  ;; label = @4
            local.get 0
            f64.abs
            f64.const 0x1p+63 (;=9.22337e+18;)
            f64.lt
            if  ;; label = @5
              local.get 0
              local.set 6
              local.get 0
              f64.abs
              f64.const 0x1p+0 (;=1;)
              f64.ge
              if  ;; label = @6
                local.get 6
                f64.const 0x0p+0 (;=0;)
                f64.gt
                if  ;; label = @7
                  local.get 6
                  f64.const 0x1p+32 (;=4.29497e+09;)
                  f64.div
                  f64.floor
                  f64.const 0x1p+32 (;=4.29497e+09;)
                  f64.const 0x1p+0 (;=1;)
                  f64.sub
                  f64.min
                  call $asm2wasm_f64_to_int
                  local.set 2
                else
                  local.get 6
                  local.get 6
                  call $asm2wasm_f64_to_int
                  f64.convert_i32_u
                  f64.sub
                  f64.const 0x1p+32 (;=4.29497e+09;)
                  f64.div
                  f64.ceil
                  call $asm2wasm_f64_to_int
                  local.set 2
                end
              else
                i32.const 0
                local.set 2
              end
              local.get 6
              call $asm2wasm_f64_to_int
              local.set 3
              br 1 (;@4;)
            end
            i32.const -2147483648
            local.set 2
            i32.const 0
            local.set 3
          end
          i32.const 0
          local.set 1
          i32.const 1
          local.set 5
          local.get 2
          local.get 1
          i32.and
          local.set 1
          local.get 7
          local.get 8
          local.get 3
          local.get 5
          i32.and
          local.get 1
          i32.or
          i32.eqz
          select
          local.set 0
          br 2 (;@1;)
        end
        local.get 0
        f64.const 0x1p-1 (;=0.5;)
        f64.add
        f64.floor
        local.set 0
        br 1 (;@1;)
      end
      local.get 6
      f64.const -0x1p-1 (;=-0.5;)
      f64.eq
      if  ;; label = @2
        local.get 4
        f64.load offset=8
        local.set 0
        local.get 0
        f64.const -0x1p+0 (;=-1;)
        f64.add
        local.set 6
        local.get 0
        local.set 7
        local.get 6
        local.set 8
        block  ;; label = @3
          local.get 0
          f64.abs
          f64.const 0x1p+63 (;=9.22337e+18;)
          f64.lt
          if  ;; label = @4
            local.get 0
            local.set 6
            local.get 0
            f64.abs
            f64.const 0x1p+0 (;=1;)
            f64.ge
            if  ;; label = @5
              local.get 6
              f64.const 0x0p+0 (;=0;)
              f64.gt
              if  ;; label = @6
                local.get 6
                f64.const 0x1p+32 (;=4.29497e+09;)
                f64.div
                f64.floor
                f64.const 0x1p+32 (;=4.29497e+09;)
                f64.const 0x1p+0 (;=1;)
                f64.sub
                f64.min
                call $asm2wasm_f64_to_int
                local.set 1
              else
                local.get 6
                local.get 6
                call $asm2wasm_f64_to_int
                f64.convert_i32_u
                f64.sub
                f64.const 0x1p+32 (;=4.29497e+09;)
                f64.div
                f64.ceil
                call $asm2wasm_f64_to_int
                local.set 1
              end
            else
              i32.const 0
              local.set 1
            end
            local.get 6
            call $asm2wasm_f64_to_int
            local.set 2
            br 1 (;@3;)
          end
          i32.const -2147483648
          local.set 1
          i32.const 0
          local.set 2
        end
        i32.const 0
        local.set 3
        i32.const 1
        local.set 5
        local.get 1
        local.get 3
        i32.and
        local.set 3
        local.get 7
        local.get 8
        local.get 2
        local.get 5
        i32.and
        local.get 3
        i32.or
        i32.eqz
        select
        local.set 0
        br 1 (;@1;)
      end
      local.get 0
      f64.const -0x1p-1 (;=-0.5;)
      f64.add
      f64.ceil
      local.set 0
    end
    local.get 4
    i32.const 16
    i32.add
    global.set 0
    local.get 0
    return)
