  (func $j80063 (type 2) (param i32 i32) (result i32)
    i32.const 0
    local.set 1
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=16
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=17
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=92
      i32.const 0
      i32.ne
      local.set 1
    end
    local.get 1
    return)
