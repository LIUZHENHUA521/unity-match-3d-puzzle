  (func $j36725 (type 0) (param i32 i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const 208
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507838
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434420
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3437580
      call $j870
      i32.const 3520228
      call $j870
      i32.const 4507838
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.const 0
    i32.store offset=200
    local.get 0
    i32.load offset=56
    local.set 3
    i32.const 3437580
    i32.load
    call $j871
    i32.const 3520228
    i32.load
    local.set 4
    block  ;; label = @1
      local.get 3
      if  ;; label = @2
        local.get 3
        i32.const 0
        local.get 4
        i32.const 0
        call $j3492
        local.get 0
        i32.load8_u offset=60
        i32.eqz
        br_if 1 (;@1;)
        i32.const 3437580
        i32.load
        call $j871
        i32.const 3520228
        i32.load
        local.set 4
      end
      i32.const 23
      i32.const 0
      local.get 4
      i32.const 0
      call $j3492
    end
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=94
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=96
      local.set 3
      block  ;; label = @2
        local.get 3
        if  ;; label = @3
          i32.const 0
          local.set 2
          local.get 3
          i32.load offset=8
          br_if 1 (;@2;)
        end
        i32.const 1
        local.set 2
      end
      local.get 2
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=96
      local.set 3
      i32.const 3434420
      i32.load
      call $j871
      local.get 3
      i32.const 0
      call $j13190
      local.set 3
      i32.const 3435416
      i32.load
      call $j871
      local.get 3
      i32.const 0
      i32.const 0
      call $j915
      i32.eqz
      br_if 0 (;@1;)
      local.get 3
      i32.const 0
      call $j944
      local.set 4
      local.get 1
      i32.const 136
      i32.add
      local.get 0
      i32.const 0
      call $j944
      i32.const 0
      call $j1028
      local.get 1
      i32.load offset=144
      local.set 2
      local.get 1
      local.get 2
      i32.store offset=192
      local.get 1
      local.get 2
      i32.store offset=80
      local.get 1
      i32.load offset=136
      local.set 2
      local.get 1
      i32.load offset=140
      local.set 5
      local.get 1
      local.get 2
      i32.store offset=184
      local.get 1
      local.get 5
      i32.store offset=188
      local.get 1
      local.get 2
      i32.store offset=72
      local.get 1
      local.get 5
      i32.store offset=76
      local.get 4
      local.get 1
      i32.const 72
      i32.add
      i32.const 0
      call $j1386
      local.get 0
      i32.load8_u offset=100
      if  ;; label = @2
        global.get 0
        i32.const 16
        i32.sub
        local.set 4
        local.get 4
        global.set 0
        local.get 4
        i32.const 0
        i32.store offset=8
        local.get 4
        local.get 3
        i32.store offset=8
        local.get 4
        i32.load offset=8
        local.set 2
        local.get 4
        i32.const 16
        i32.add
        global.set 0
        local.get 1
        local.get 2
        i32.store offset=200
        local.get 0
        i32.load offset=112
        local.set 5
        local.get 0
        i32.load offset=116
        local.set 2
        local.get 5
        local.set 4
        local.get 1
        i32.const -64
        i32.sub
        local.set 5
        local.get 5
        local.get 4
        i32.store
        local.get 5
        local.get 2
        i32.store offset=4
        local.get 0
        i32.load offset=104
        local.set 2
        local.get 0
        i32.load offset=108
        local.set 5
        local.get 1
        local.get 2
        i32.store offset=56
        local.get 1
        local.get 5
        i32.store offset=60
        local.get 1
        i32.const 136
        i32.add
        local.get 1
        i32.const 56
        i32.add
        i32.const 0
        call $j19551
        local.get 1
        i32.const 88
        i32.add
        local.get 1
        i32.const 136
        i32.add
        i32.const 44
        call $j883
        drop
        local.get 1
        i32.const 12
        i32.add
        local.get 1
        i32.const 88
        i32.add
        i32.const 44
        call $j883
        drop
        local.get 1
        i32.const 200
        i32.add
        local.get 1
        i32.const 12
        i32.add
        i32.const 0
        call $j13173
      end
      local.get 3
      i32.const 0
      call $j3343
    end
    local.get 0
    i32.load offset=124
    local.set 3
    i32.const 3435416
    i32.load
    call $j871
    local.get 3
    i32.const 0
    call $j982
    if  ;; label = @1
      local.get 0
      i32.load offset=124
      local.set 0
      local.get 0
      local.set 2
      local.get 0
      i32.load
      i32.const -64
      i32.sub
      local.set 0
      local.get 0
      i32.load offset=188
      local.get 2
      local.get 0
      i32.load offset=192
      call $redir_vii
    end
    local.get 1
    i32.const 208
    i32.add
    global.set 0)
