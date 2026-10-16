  (func $j17948 (type 16) (param i32 f32 i32)
    i32.const 4508437
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3437580
      call $j870
      i32.const 4508437
      i32.const 1
      i32.store8
    end
    i32.const 3437580
    i32.load
    call $j871
    i32.const 3437580
    i32.load
    i32.load offset=92
    local.get 1
    f32.store offset=40
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            i32.const 3437580
            i32.load
            i32.load offset=92
            i32.load offset=28
            i32.const 1
            i32.sub
            br_table 0 (;@4;) 1 (;@3;) 3 (;@1;)
          end
          i32.const 3437580
          i32.load
          call $j871
          i32.const 3437580
          i32.load
          i32.load offset=92
          i32.load
          i32.load offset=36
          local.set 0
          i32.const 3437580
          i32.load
          i32.load offset=92
          i32.load
          i32.const 40
          i32.add
          local.set 2
          br 1 (;@2;)
        end
        i32.const 3437580
        i32.load
        call $j871
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.load offset=36
        local.set 0
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.const 44
        i32.add
        local.set 2
      end
      local.get 0
      local.get 2
      f32.load
      i32.const 3437580
      i32.load
      i32.load offset=92
      f32.load offset=40
      f32.mul
      i32.const 0
      call $j8410
    end)
