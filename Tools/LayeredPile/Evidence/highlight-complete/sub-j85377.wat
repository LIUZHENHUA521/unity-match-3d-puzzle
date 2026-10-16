  (func $j85377 (type 3) (param i32 i32 i32)
    i32.const 4505030
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505030
      i32.const 1
      i32.store8
    end
    i32.const 3435416
    i32.load
    i32.const 871
    call_indirect (type 0)
    local.get 1
    i32.const 0
    i32.const 0
    i32.const 915
    call_indirect (type 5)
    if  ;; label = @1
      local.get 1
      local.get 0
      i32.const 0
      call $j6735 ;; HighlightProfile.Save
    end)
