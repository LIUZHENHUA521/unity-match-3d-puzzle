  (func $j8125 (type 0) (param i32 i32)
    (local i32 i32 i32)
    i32.const 4505042
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3485580
      call $j870 ;; 
      i32.const 3485664
      call $j870 ;; 
      i32.const 3485880
      call $j870 ;; 
      i32.const 3432484
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3427916
      call $j870 ;; 
      i32.const 4505042
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=424
    local.set 1
    i32.const 3435416
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      local.get 1
      i32.const 0
      i32.const 0
      call $j924 ;; 
      br_if 0 (;@1;)
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load
      local.set 1
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 1
      i32.const 0
      i32.const 0
      call $j924 ;; 
      br_if 0 (;@1;)
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                block  ;; label = @7
                  block  ;; label = @8
                    local.get 0
                    i32.load offset=24
                    br_table 4 (;@4;) 0 (;@8;) 1 (;@7;) 2 (;@6;) 3 (;@5;) 7 (;@1;) 5 (;@3;) 6 (;@2;)
                  end
                  local.get 0
                  i32.load offset=424
                  i32.const 3485664
                  i32.load
                  call $j1084 ;; 
                  local.set 1
                  i32.const 3435416
                  i32.load
                  call $j871 ;; 
                  local.get 1
                  i32.const 0
                  i32.const 0
                  call $j915 ;; 
                  i32.eqz
                  br_if 5 (;@2;)
                  i32.const 3427916
                  i32.load
                  i32.const 1
                  call $j888 ;; 
                  local.set 2
                  local.get 2
                  local.get 1
                  call $j877 ;; 
                  local.get 2
                  local.get 1
                  i32.store offset=16
                  br 5 (;@2;)
                end
                local.get 0
                i32.load offset=424
                local.set 1
                loop  ;; label = @7
                  block  ;; label = @8
                    local.get 1
                    i32.const 0
                    call $j1175 ;; 
                    local.set 2
                    i32.const 3435416
                    i32.load
                    call $j871 ;; 
                    local.get 2
                    i32.const 0
                    i32.const 0
                    call $j915 ;; 
                    i32.eqz
                    br_if 0 (;@8;)
                    local.get 1
                    i32.const 0
                    call $j1175 ;; 
                    local.set 1
                    br 1 (;@7;)
                  end
                end
                local.get 1
                i32.const 3485880
                i32.load
                call $j2964 ;; 
                local.set 2
                br 4 (;@2;)
              end
              local.get 0
              i32.load offset=424
              local.set 2
              local.get 0
              i32.const 0
              call $j944 ;; 
              local.set 3
              i32.const 3435416
              i32.load
              call $j871 ;; 
              local.get 0
              local.set 1
              local.get 1
              local.set 4
              local.get 2
              local.get 3
              i32.const 0
              call $j915 ;; 
              if  ;; label = @6
                local.get 0
                i32.load offset=424
                i32.const 3485580
                i32.load
                call $j1084 ;; 
                local.set 1
                i32.const 3435416
                i32.load
                call $j871 ;; 
                local.get 1
                local.get 0
                local.get 1
                i32.const 0
                i32.const 0
                call $j915 ;; 
                select
                local.set 1
              end
              i32.const 20488
              local.get 4
              local.get 1
              i32.load offset=28
              local.get 1
              call $redir_iiii
              local.set 2
              br 3 (;@2;)
            end
            local.get 0
            i32.load offset=424
            local.set 2
            local.get 0
            i32.const 0
            call $j944 ;; 
            local.set 3
            i32.const 3435416
            i32.load
            call $j871 ;; 
            local.get 0
            local.set 1
            local.get 1
            local.set 4
            local.get 2
            local.get 3
            i32.const 0
            call $j915 ;; 
            if  ;; label = @5
              local.get 0
              i32.load offset=424
              i32.const 3485580
              i32.load
              call $j1084 ;; 
              local.set 1
              i32.const 3435416
              i32.load
              call $j871 ;; 
              local.get 1
              local.get 0
              local.get 1
              i32.const 0
              i32.const 0
              call $j915 ;; 
              select
              local.set 1
            end
            i32.const 20489
            local.get 4
            local.get 1
            i32.load offset=28
            local.get 1
            call $redir_iiii
            local.set 2
            br 2 (;@2;)
          end
          local.get 0
          i32.load offset=424
          i32.const 3485880
          i32.load
          call $j2964 ;; 
          local.set 2
          br 1 (;@2;)
        end
        local.get 0
        i32.load offset=424
        i32.const 3485664
        i32.load
        call $j1084 ;; 
        local.set 1
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 0
        i32.const 0
        call $j915 ;; 
        if  ;; label = @3
          i32.const 3427916
          i32.load
          i32.const 1
          call $j888 ;; 
          local.set 2
          local.get 2
          local.get 1
          call $j877 ;; 
          local.get 2
          local.get 1
          i32.store offset=16
          br 1 (;@2;)
        end
        local.get 0
        i32.load offset=424
        i32.const 0
        call $j1175 ;; 
        local.set 1
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 0
        i32.const 0
        call $j915 ;; 
        i32.eqz
        br_if 0 (;@2;)
        local.get 0
        i32.load offset=424
        i32.const 0
        call $j1175 ;; 
        i32.const 3485664
        i32.load
        call $j1084 ;; 
        local.set 1
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 0
        i32.const 0
        call $j915 ;; 
        i32.eqz
        br_if 0 (;@2;)
        i32.const 3427916
        i32.load
        i32.const 1
        call $j888 ;; 
        local.set 2
        local.get 0
        i32.load offset=424
        i32.const 0
        call $j1175 ;; 
        i32.const 3485664
        i32.load
        call $j1084 ;; 
        local.set 1
        local.get 2
        local.get 1
        call $j877 ;; 
        local.get 2
        local.get 1
        i32.store offset=16
      end
      local.get 0
      local.get 2
      local.get 1
      call $j20231 ;; HighlightEffect.SetupMaterial
    end)
