  (func $j5908 (type 3) (param i32 i32 i32)
    block  ;; label = @1
      local.get 1
      i32.eqz
      if  ;; label = @2
        i32.const 0
        i32.const 1337
        call_indirect (type 1)
        br_if 1 (;@1;)
      end
      local.get 0
      i32.const 0
      i32.store8 offset=748
      local.get 0
      local.get 0
      i32.const 9989
      call_indirect (type 4)
      return
    end
    local.get 0
    i32.const 1
    i32.store8 offset=748)
