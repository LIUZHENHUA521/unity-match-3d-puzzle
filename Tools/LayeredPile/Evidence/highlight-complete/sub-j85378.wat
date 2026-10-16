  (func $j85378 (type 4) (param i32 i32)
    i32.const 4505031
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505031
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=12
    local.set 1
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
      local.get 0
      i32.load offset=12
      local.get 0
      i32.const 0
      call $j6735 ;; HighlightProfile.Save
    end)
