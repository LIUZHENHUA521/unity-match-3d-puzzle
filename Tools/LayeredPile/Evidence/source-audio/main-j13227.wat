  (func $j13227 (type 11) (param i32 i32 i32 i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 i32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 6
    local.get 6
    global.set 0
    i32.const 4509370
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3470236
      call $j870
      i32.const 3470252
      call $j870
      i32.const 3470256
      call $j870
      i32.const 3470264
      call $j870
      i32.const 3472460
      call $j870
      i32.const 3470268
      call $j870
      i32.const 3472464
      call $j870
      i32.const 4509370
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871
    block  ;; label = @1
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load8_u offset=36
      if  ;; label = @2
        i32.const 4525920
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4525920
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 5
        br 1 (;@1;)
      end
      local.get 3
      i32.const 1
      i32.eq
      local.set 5
      local.get 1
      i32.const 192
      i32.const 208
      local.get 5
      select
      i32.add
      i32.load
      local.set 9
      local.get 1
      i32.const 188
      i32.const 204
      local.get 5
      select
      i32.add
      i32.load
      local.set 10
      local.get 2
      i32.load8_u offset=18
      local.set 12
      local.get 4
      if  ;; label = @2
        local.get 6
        i32.const 16
        i32.add
        local.get 2
        i32.const 0
        call $j944
        i32.const 0
        call $j1028
        local.get 1
        local.get 6
        f32.load offset=20
        i32.const 0
        local.get 3
        local.get 5
        call $j9878
      end
      i32.const 4525920
      i32.load8_u
      local.set 5
      i32.const 3470264
      i32.load
      drop
      i32.const 3472460
      i32.load
      drop
      local.get 10
      i32.load offset=12
      local.get 1
      i32.load offset=184
      i32.load offset=12
      i32.ge_s
      if  ;; label = @2
        local.get 5
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4525920
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 5
        br 1 (;@1;)
      end
      local.get 5
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        call $j870
        i32.const 4525920
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 5
      local.get 6
      local.get 5
      i32.load offset=8
      i32.store offset=24
      local.get 5
      i32.load
      local.set 8
      local.get 5
      i32.load offset=4
      local.set 7
      local.get 6
      local.get 8
      i32.store offset=16
      local.get 6
      local.get 7
      i32.store offset=20
      local.get 2
      i32.const 0
      call $j944
      local.get 1
      i32.load offset=124
      i32.const 0
      call $j1628
      local.get 2
      i32.const 1
      local.get 1
      f32.load offset=584
      i32.const 0
      call $j12985
      local.get 4
      if  ;; label = @2
        local.get 2
        i32.const 0
        call $j19260
      end
      local.get 9
      local.get 2
      i32.const 3470256
      i32.load
      call $j1342
      drop
      i32.const 0
      local.set 5
      i32.const 0
      local.set 9
      loop  ;; label = @2
        i32.const 3470264
        i32.load
        drop
        local.get 5
        local.get 10
        i32.load offset=12
        i32.lt_s
        if  ;; label = @3
          local.get 10
          local.get 5
          i32.const 3470268
          i32.load
          call $j895
          i32.load offset=12
          local.get 2
          i32.load offset=12
          i32.const 0
          call $j899
          local.set 8
          local.get 5
          local.get 9
          local.get 8
          select
          local.set 9
          local.get 11
          local.get 8
          i32.or
          local.set 11
          local.get 5
          i32.const 1
          i32.add
          local.set 5
          br 1 (;@2;)
        end
      end
      block  ;; label = @2
        local.get 11
        i32.const 1
        i32.and
        if  ;; label = @3
          local.get 10
          local.get 9
          i32.const 1
          i32.add
          local.get 2
          i32.const 3470252
          i32.load
          call $j2331
          i32.const 3434312
          i32.load
          call $j871
          local.get 5
          call $j1923
          br_if 1 (;@2;)
          i32.const 3472460
          i32.load
          drop
          local.get 1
          i32.load offset=184
          i32.load offset=12
          local.get 9
          i32.const 1
          i32.add
          i32.le_s
          br_if 1 (;@2;)
          local.get 6
          local.get 1
          i32.load offset=184
          local.get 9
          i32.const 1
          i32.add
          i32.const 3472464
          i32.load
          call $j895
          i32.const 0
          call $j1028
          local.get 6
          local.get 6
          i32.load offset=8
          i32.store offset=24
          local.get 6
          i32.load
          local.set 7
          local.get 6
          i32.load offset=4
          local.set 8
          local.get 6
          local.get 7
          i32.store offset=16
          local.get 6
          local.get 8
          i32.store offset=20
          br 1 (;@2;)
        end
        local.get 10
        local.get 2
        i32.const 3470236
        i32.load
        call $j903
        i32.const 3434312
        i32.load
        call $j871
        local.get 5
        call $j1923
        br_if 0 (;@2;)
        i32.const 3472460
        i32.load
        drop
        local.get 1
        i32.load offset=184
        i32.load offset=12
        i32.const 1
        i32.lt_s
        br_if 0 (;@2;)
        local.get 1
        i32.load offset=184
        local.set 5
        i32.const 3472460
        i32.load
        drop
        local.get 6
        local.get 5
        local.get 5
        i32.load offset=12
        i32.const 1
        i32.sub
        i32.const 3472464
        i32.load
        call $j895
        i32.const 0
        call $j1028
        local.get 6
        local.get 6
        i32.load offset=8
        i32.store offset=24
        local.get 6
        i32.load
        local.set 8
        local.get 6
        i32.load offset=4
        local.set 7
        local.get 6
        local.get 8
        i32.store offset=16
        local.get 6
        local.get 7
        i32.store offset=20
      end
      i32.const 3434312
      i32.load
      call $j871
      block  ;; label = @2
        local.get 5
        call $j5871
        if  ;; label = @3
          local.get 1
          local.get 1
          local.get 3
          local.get 1
          i32.load offset=628
          local.get 2
          local.get 3
          local.get 12
          i32.const 0
          i32.ne
          i32.const 0
          call $j38478
          local.get 5
          call $j19875
          i32.const 0
          call $j1135
          drop
          br 1 (;@2;)
        end
        local.get 1
        local.get 3
        i32.const 1
        local.get 5
        call $j9879
      end
      local.get 1
      i32.const 0
      i32.store offset=812
      local.get 2
      i32.load8_u offset=18
      if  ;; label = @2
        local.get 1
        local.get 2
        local.get 5
        call $j38487
      end
      local.get 6
      i32.const 16
      i32.add
      local.set 5
      local.get 4
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3428556
      i32.load
      call $j871
      i32.const 3428556
      i32.load
      i32.load offset=92
      i32.load offset=176
      local.set 1
      local.get 1
      i32.eqz
      br_if 0 (;@1;)
      local.get 2
      i32.const 0
      call $j925
      i32.const 0
      call $j1073
      local.set 2
      local.get 1
      i32.load offset=12
      local.get 1
      i32.load offset=32
      local.get 2
      local.get 1
      i32.load offset=20
      call $redir_viii
    end
    local.get 5
    i32.load
    local.set 7
    local.get 5
    i32.load offset=4
    local.set 8
    local.get 7
    local.set 1
    local.get 0
    local.set 7
    local.get 7
    local.get 1
    i32.store
    local.get 7
    local.get 8
    i32.store offset=4
    local.get 7
    local.get 5
    i32.load offset=8
    i32.store offset=8
    local.get 6
    i32.const 32
    i32.add
    global.set 0)
