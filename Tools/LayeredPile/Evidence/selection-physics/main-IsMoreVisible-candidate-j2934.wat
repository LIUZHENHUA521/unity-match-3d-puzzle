  (func $j2934 (type 7) (param i32 i32 i32 i32)
    (local i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    local.get 1
    i32.store8 offset=15
    i32.const 4507071
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3452608
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3469628
      call $j870
      i32.const 3472460
      call $j870
      i32.const 3469632
      call $j870
      i32.const 3472464
      call $j870
      i32.const 3434604
      call $j870
      i32.const 3435224
      call $j870
      i32.const 3543288
      call $j870
      i32.const 3520228
      call $j870
      i32.const 4507071
      i32.const 1
      i32.store8
    end
    local.get 1
    if  ;; label = @1
      local.get 0
      local.get 0
      call $j38083
    end
    local.get 0
    i32.load8_u offset=149
    i32.eqz
    if  ;; label = @1
      local.get 0
      local.get 0
      call $j38080
      local.get 0
      i32.const 35
      i32.const 5
      local.get 0
      call $j38082
      local.get 0
      i32.const 1
      i32.store8 offset=149
    end
    local.get 0
    i32.load offset=124
    local.set 1
    local.get 1
    local.set 4
    local.get 1
    i32.load
    local.set 1
    local.get 1
    i32.load offset=220
    local.get 4
    i32.const 0
    local.get 1
    i32.const 32
    i32.add
    i32.load offset=192
    call $redir_viii
    i32.const 3434604
    i32.load
    i32.load offset=92
    i32.const 0
    i32.store8 offset=48
    local.get 3
    i32.const 15
    i32.add
    i32.const 0
    call $j2713
    local.set 1
    local.get 1
    local.set 4
    i32.const 3520228
    i32.load
    local.set 1
    i32.const 3543288
    i32.load
    local.get 4
    local.get 1
    local.get 1
    local.get 1
    local.get 1
    i32.const 0
    call $j1051
    i32.const 0
    call $j1547
    local.get 3
    i32.load8_u offset=15
    local.set 1
    local.get 0
    local.get 1
    i32.store8 offset=151
    local.get 0
    i32.load offset=16
    local.get 1
    i32.const 0
    i32.const 0
    call $j1549
    local.get 3
    i32.load8_u offset=15
    local.get 0
    call $j19735
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=148
      i32.eqz
      br_if 0 (;@1;)
      block  ;; label = @2
        block  ;; label = @3
          local.get 3
          i32.load8_u offset=15
          i32.eqz
          if  ;; label = @4
            i32.const 0
            local.set 1
            br 1 (;@3;)
          end
          local.get 0
          i32.const 0
          local.get 0
          call $j38078
          i32.const 0
          local.set 1
          loop  ;; label = @4
            i32.const 3469628
            i32.load
            drop
            local.get 1
            local.get 0
            i32.load offset=28
            i32.load offset=12
            i32.lt_s
            if  ;; label = @5
              local.get 0
              i32.load offset=28
              local.get 1
              i32.const 3469632
              i32.load
              call $j895
              i32.const 1
              i32.const 0
              call $j19734
              local.get 0
              i32.load offset=28
              local.get 1
              i32.const 3469632
              i32.load
              call $j895
              i32.const 0
              i32.const 0
              call $j9844
              local.get 1
              i32.const 1
              i32.add
              local.set 1
              br 1 (;@4;)
            end
          end
          local.get 0
          i32.load offset=48
          i32.const 3452608
          i32.load
          call $j1167
          local.get 0
          i32.load8_u offset=148
          if  ;; label = @4
            local.get 0
            local.get 0
            call $j19736
          end
          i32.const 4508592
          i32.load8_u
          i32.eqz
          if  ;; label = @4
            i32.const 3432052
            call $j870
            i32.const 4508592
            i32.const 1
            i32.store8
          end
          i32.const 3432052
          i32.load
          i32.load offset=92
          i32.load offset=16
          br_if 1 (;@2;)
          local.get 0
          i32.load8_u offset=148
          i32.eqz
          br_if 1 (;@2;)
          local.get 0
          i32.load8_u offset=150
          i32.eqz
          br_if 1 (;@2;)
          local.get 0
          local.get 0
          call $j13171
          br 1 (;@2;)
        end
        loop  ;; label = @3
          i32.const 3469628
          i32.load
          drop
          local.get 1
          local.get 0
          i32.load offset=28
          i32.load offset=12
          i32.ge_s
          br_if 1 (;@2;)
          local.get 0
          i32.load offset=28
          local.get 1
          i32.const 3469632
          i32.load
          call $j895
          i32.const 0
          i32.const 0
          call $j19734
          local.get 1
          i32.const 1
          i32.add
          local.set 1
          br 0 (;@3;)
        end
        unreachable
      end
      i32.const 3434604
      i32.load
      i32.load offset=92
      i32.const 0
      i32.store offset=20
      local.get 0
      i32.const 0
      i32.store8 offset=277
      i32.const 3435224
      i32.load
      call $j871
      i32.const 4509443
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3435224
        call $j870
        i32.const 4509443
        i32.const 1
        i32.store8
      end
      i32.const 3435224
      i32.load
      call $j871
      i32.const 3435224
      i32.load
      i32.load offset=92
      i32.load offset=28
      i32.load offset=24
      i32.const 0
      call $j13040
      local.set 1
      i32.const 3472460
      i32.load
      drop
      local.get 1
      local.get 0
      i32.load offset=64
      i32.load offset=12
      i32.le_s
      if  ;; label = @2
        i32.const 3434604
        i32.load
        i32.load offset=92
        i32.const 1
        i32.store8 offset=4
        local.get 3
        local.get 0
        i32.load offset=64
        local.get 1
        i32.const 1
        i32.sub
        i32.const 3472464
        i32.load
        call $j895
        i32.const 0
        call $j944
        i32.const 0
        call $j1028
        i32.const 3434604
        i32.load
        i32.load offset=92
        local.get 3
        f32.load offset=8
        f32.store offset=8
      end
      local.get 0
      local.get 0
      call $j13172
      i32.const 3435224
      i32.load
      call $j871
      i32.const 4509443
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3435224
        call $j870
        i32.const 4509443
        i32.const 1
        i32.store8
      end
      i32.const 3435224
      i32.load
      call $j871
      i32.const 3435224
      i32.load
      i32.load offset=92
      i32.load offset=28
      i32.load8_u offset=41
      local.get 2
      i32.and
      if  ;; label = @2
        local.get 0
        i32.const 1
        i32.const 1
        local.get 0
        call $j9845
      end
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load8_u offset=6
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.const 0
      i32.store8 offset=6
      i32.const 3435224
      i32.load
      call $j871
      i32.const 4509443
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3435224
        call $j870
        i32.const 4509443
        i32.const 1
        i32.store8
      end
      i32.const 3435224
      i32.load
      call $j871
      i32.const 3435224
      i32.load
      i32.load offset=92
      i32.load offset=28
      i32.load8_u offset=41
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.const 1
      i32.const 1
      local.get 0
      call $j9845
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0)
