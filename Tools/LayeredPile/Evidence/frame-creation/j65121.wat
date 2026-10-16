  (func $j65121 (type 2) (param i32 i32) (result i32)
    (local i32)
    i32.const 4507769
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3438764
      call $j870
      i32.const 4507769
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=8
    local.set 2
    local.get 2
    i32.const 2
    i32.le_u
    if  ;; label = @1
      local.get 0
      i32.load offset=16
      local.set 1
      local.get 0
      i32.const -1
      i32.store offset=8
      block  ;; label = @2
        local.get 1
        i32.load8_u offset=60
        if  ;; label = @3
          i32.const 3438764
          i32.load
          call $j871
          local.get 0
          i32.const 3438764
          i32.load
          i32.load offset=92
          i32.load offset=8
          i32.store offset=12
          i32.const 1
          local.set 1
          br 1 (;@2;)
        end
        local.get 1
        i32.const 0
        call $j5870
        local.get 1
        i32.const 0
        call $j19882
        i32.const 3438764
        i32.load
        call $j871
        local.get 0
        i32.const 3438764
        i32.load
        i32.load offset=92
        i32.load offset=8
        i32.store offset=12
        i32.const 2
        local.set 1
      end
      local.get 0
      local.get 1
      i32.store offset=8
    end
    local.get 2
    i32.const 3
    i32.lt_u
    return)